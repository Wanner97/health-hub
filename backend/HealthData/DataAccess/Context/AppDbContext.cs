using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ImportBatch> ImportBatches { get; set; }
        public DbSet<ActivityDay> ActivityDays { get; set; }
        public DbSet<SleepSession> SleepSessions { get; set; }
        public DbSet<SleepStage> SleepStages { get; set; }
        public DbSet<HeartRateDay> HeartRateDays { get; set; }
        public DbSet<HeartRateHourlyRecord> HeartRateHourlyRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
