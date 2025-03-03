using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMSRateLimiter.Application.DTOs;
using SMSRateLimiter.Domain.Entities;
using SMSRateLimiter.Infrastructure.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Api.BackgoundServices
{
    public class SmsLogBatchWriter : BackgroundService
    {
        private readonly ConcurrentQueue<SmsLogDto> _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SmsLogBatchWriter> _logger;

        public SmsLogBatchWriter(
            ConcurrentQueue<SmsLogDto> queue,
            IServiceScopeFactory scopeFactory,
            ILogger<SmsLogBatchWriter> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var batch = new List<SmsLog>();
                    while (_queue.TryDequeue(out var log))
                    {
                        batch.Add(new SmsLog
                        {
                            AccountId = log.AccountId,
                            PhoneNumber = log.PhoneNumber,
                            Timestamp = log.Timestamp
                        });

                        if (batch.Count >= 100) // Limit batch size
                            break;
                    }

                    if (batch.Count > 0)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<MetricsDbContext>();

                        await dbContext.SmsLogs.AddRangeAsync(batch, stoppingToken);
                        await dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Batch of {Count} SMS logs saved.", batch.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while writing SMS log batch.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Wait between batches
            }
        }
    }
}
