
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
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IClock, SystemClock>();

            // Caching Registration Options:
            // Option 1: In-Memory Caching (for development or single-instance scenarios)
           
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IRateLimitCache, MemoryRateLimitCache>();

            // Register the Domain rate limiter with desired limits (e.g., 5 per number, 100 globally)
            builder.Services.AddSingleton<ISmsRateLimiter>(sp =>
            {
                var cache = sp.GetRequiredService<IRateLimitCache>();
                var systemClock = sp.GetRequiredService<IClock>();
                return new SmsRateLimiter(cache, systemClock, maxPerNumber: 5, maxGlobal: 100);
            });

            // Register the Application service
            builder.Services.AddSingleton<ISmsRateLimiterAppService, SmsRateLimiterAppService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
