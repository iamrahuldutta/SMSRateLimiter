
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SMSRateLimiter.Api.Filters;
using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Application.Implementations.Services;
using SMSRateLimiter.Domain.Contracts.Caching;
using SMSRateLimiter.Domain.Contracts.Services;
using SMSRateLimiter.Domain.Implementations.Services;
using SMSRateLimiter.Infrastructure.Implementations.Caching;
using FluentValidation;
using FluentValidation.AspNetCore;
using SMSRateLimiter.Application.Validations;
using StackExchange.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SMSRateLimiter.Infrastructure.Persistence.Repository;
using SMSRateLimiter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SMSRateLimiter.Domain.Contracts.Repositories;
using SMSRateLimiter.Api.BackgoundServices;
using SMSRateLimiter.Application.DTOs;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using SMSRateLimiter.Domain.Models;

namespace SMSRateLimiter.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog for logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Add CORS services
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Configure EF Core to use SQLite.
            builder.Services.AddDbContext<MetricsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton(new ConcurrentQueue<SmsLogDto>());
            builder.Services.AddSingleton<ISmsLogBatchLogger, SmsLogBatchLogger>();
            builder.Services.AddHostedService<SmsLogBatchWriter>();

            // Add services to the DI container
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidationFilter>();
            });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<SmsRequestValidator>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });



            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            builder.Services.AddScoped<ISmsLogRepository, SmsLogRepository>();

            builder.Services.AddSingleton<IClock, SystemClock>();

            // Caching Registration Options:
            // Option 1: In-Memory Caching (for development or single-instance scenarios)

            //builder.Services.AddMemoryCache();
            //builder.Services.AddSingleton<IRateLimitCache, MemoryRateLimitCache>();


            // Option 2: Redis Caching (for distributed scenarios)
            // Uncomment the following lines and comment out the in-memory registration above
            // Make sure you have a valid connection string in your configuration (e.g., appsettings.json)
            // otherwise the fallback mechanism will use In-MemoryCache

            string redisConnection = builder.Configuration.GetConnectionString("Redis");

            try
            {
                // Attempt to connect to Redis
                var redisMultiplexer = ConnectionMultiplexer.Connect(redisConnection);
                builder.Services.AddSingleton<IConnectionMultiplexer>(redisMultiplexer);
                builder.Services.AddSingleton<IRateLimitCache, RedisRateLimitCache>();
            }
            catch (Exception ex)
            {
                // Log the error and fallback to in-memory caching
                Log.Error(ex, "Redis connection failed. Falling back to in-memory caching.");
                builder.Services.AddMemoryCache();
                builder.Services.AddSingleton<IRateLimitCache, MemoryRateLimitCache>();
            }

            // Register health checks
            builder.Services.AddHealthChecks()
                // This health check uses the connection string directly. It will attempt to connect and perform a ping.
                .AddRedis(redisConnection, name: "redis", failureStatus: HealthStatus.Unhealthy, tags: new[] { "db", "redis" });

            builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimitOptions"));


            // Register the Domain rate limiter with desired limits (e.g., 5 per number, 100 globally)
            builder.Services.AddSingleton<ISmsRateLimiter>(sp =>
            {
                var cache = sp.GetRequiredService<IRateLimitCache>();
                var options = sp.GetRequiredService<IOptions<RateLimitOptions>>();
                return new SmsRateLimiter(cache, options);
            });

            // Register the Application service
            builder.Services.AddSingleton<ISmsRateLimiterAppService, SmsRateLimiterAppService>();

            var app = builder.Build();
            app.UseCors("AllowReactApp");
            // Enable middleware for health checks
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    // Customize the health check response (e.g., JSON response)
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new {
                            key = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();



            // Use routing and map controllers
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
