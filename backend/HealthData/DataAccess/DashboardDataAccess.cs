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
                        .AsNoTracking()
                        .OrderByDescending(x => x.ImportedAtUtc)
                        .FirstOrDefault(),

                    LatestActivityDay = context.ActivityDays
                        .AsNoTracking()
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefault(),

                    LatestSleepSession = context.SleepSessions
                        .AsNoTracking()
                        .OrderByDescending(x => x.StartTimeUtc)
                        .FirstOrDefault(),

                    LatestHeartRateDay = context.HeartRateDays
                        .AsNoTracking()
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefault(),

                    LatestBloodOxygenDay = context.BloodOxygenDays
                        .AsNoTracking()
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefault(),

                    LatestHeightMeasurement = context.HeightMeasurements
                        .AsNoTracking()
                        .OrderByDescending(x => x.MeasuredAtUtc)
                        .FirstOrDefault(),

                    LatestWeightMeasurement = context.WeightMeasurements
                        .AsNoTracking()
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