using Microsoft.EntityFrameworkCore;
using SMSRateLimiter.Domain.Contracts.Repositories;
using SMSRateLimiter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Infrastructure.Persistence.Repository
{
    public class SmsLogRepository : ISmsLogRepository
    {
        private readonly MetricsDbContext _context;

        public SmsLogRepository(MetricsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SmsLog>> GetLogsAsync(int accountId, string? phoneNumber = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.SmsLogs.AsQueryable();

            query = query.Where(x => x.AccountId == accountId);

            if (!string.IsNullOrWhiteSpace(phoneNumber))
                query = query.Where(x => x.PhoneNumber == phoneNumber);

            if (from.HasValue)
                query = query.Where(x => x.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.Timestamp <= to.Value);

            return await query.OrderByDescending(x => x.Timestamp).ToListAsync();
        }
    }
}
