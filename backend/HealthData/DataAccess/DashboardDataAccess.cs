using Common.Dashboard;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class DashboardDataAccess : IDashboardDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public DashboardDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public HomepageDashboardData GetHomepageDashboardData()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return new HomepageDashboardData
                {
                    LatestImportBatch = context.ImportBatches
                        .OrderByDescending(x => x.ImportedAtUtc)
                        .FirstOrDefault(),

                    LatestActivityDay = context.ActivityDays
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefault(),

                    LatestSleepSession = context.SleepSessions
                        .OrderByDescending(x => x.StartTimeUtc)
                        .FirstOrDefault(),

                    LatestHeartRateDay = context.HeartRateDays
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefault(),

                    LatestBloodOxygenDay = context.BloodOxygenDays
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefault(),

                    LatestHeightMeasurement = context.HeightMeasurements
                        .OrderByDescending(x => x.MeasuredAtUtc)
                        .FirstOrDefault(),

                    LatestWeightMeasurement = context.WeightMeasurements
                        .OrderByDescending(x => x.MeasuredAtUtc)
                        .FirstOrDefault(),

                    LatestNutritionDay = context.NutritionDays
                        .AsNoTracking()
                        .Include(x => x.NutrientTotals)
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefault()
                };
            }
        }
    }
}