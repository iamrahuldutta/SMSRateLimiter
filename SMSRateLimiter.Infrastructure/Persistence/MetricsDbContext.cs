using Microsoft.EntityFrameworkCore;
using SMSRateLimiter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Infrastructure.Persistence
{
    public class MetricsDbContext : DbContext
    {
        public MetricsDbContext(DbContextOptions<MetricsDbContext> options)
            : base(options)
        {
        }

        public DbSet<SmsLog> SmsLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the AllowedSmsLog entity.
            modelBuilder.Entity<SmsLog>(entity =>
            {
                // Table name
                entity.ToTable("SmsLogs");

                // Primary Key
                entity.HasKey(e => e.Id);

                // AccountId - Required
                entity.Property(e => e.AccountId)
                      .IsRequired();

                // PhoneNumber - Required, with max length
                entity.Property(e => e.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(16); // Adjust length as per your expected phone number formats

                // Timestamp - Required
                entity.Property(e => e.Timestamp)
                      .IsRequired();

                // Composite index on AccountId, PhoneNumber, Timestamp
                entity.HasIndex(e => new { e.AccountId, e.PhoneNumber, e.Timestamp });
            });

        }
    }
}
