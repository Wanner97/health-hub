using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Import
{
    public class HealthImportExistingDataLoader : IHealthImportExistingDataLoader
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public HealthImportExistingDataLoader(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public Dictionary<DateOnly, ActivityDay> GetExistingActivityDays(string source, IEnumerable<DateOnly> dates)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctDates = dates
                    .Distinct()
                    .ToList();

                return context.ActivityDays
                    .Where(x => x.Source == source && distinctDates.Contains(x.Date))
                    .ToDictionary(x => x.Date, x => x);
            }
        }

        public Dictionary<DateTime, SleepSession> GetExistingSleepSessions(string source, IEnumerable<DateTime> startTimes)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctStartTimes = startTimes
                    .Distinct()
                    .ToList();

                return context.SleepSessions
                    .Include(x => x.SleepStages)
                    .Where(x => x.Source == source && distinctStartTimes.Contains(x.StartTimeUtc))
                    .ToDictionary(x => x.StartTimeUtc, x => x);
            }
        }

        public Dictionary<DateOnly, HeartRateDay> GetExistingHeartRateDays(string source, IEnumerable<DateOnly> dates)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctDates = dates
                    .Distinct()
                    .ToList();

                return context.HeartRateDays
                    .Include(x => x.HourlyRecords)
                    .Where(x => x.Source == source && distinctDates.Contains(x.Date))
                    .ToDictionary(x => x.Date, x => x);
            }
        }

        public Dictionary<DateOnly, BloodOxygenDay> GetExistingBloodOxygenDays(string source, IEnumerable<DateOnly> dates)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctDates = dates
                    .Distinct()
                    .ToList();

                return context.BloodOxygenDays
                    .Where(x => x.Source == source && distinctDates.Contains(x.Date))
                    .ToDictionary(x => x.Date, x => x);
            }
        }

        public Dictionary<DateTime, HeightMeasurement> GetExistingHeightMeasurements(string source, IEnumerable<DateTime> measuredAtUtcValues)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctMeasuredAtUtcValues = measuredAtUtcValues
                    .Distinct()
                    .ToList();

                return context.HeightMeasurements
                    .Where(x => x.Source == source && distinctMeasuredAtUtcValues.Contains(x.MeasuredAtUtc))
                    .ToDictionary(x => x.MeasuredAtUtc, x => x);
            }
        }

        public Dictionary<DateTime, WeightMeasurement> GetExistingWeightMeasurements(string source, IEnumerable<DateTime> measuredAtUtcValues)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctMeasuredAtUtcValues = measuredAtUtcValues
                    .Distinct()
                    .ToList();

                return context.WeightMeasurements
                    .Where(x => x.Source == source && distinctMeasuredAtUtcValues.Contains(x.MeasuredAtUtc))
                    .ToDictionary(x => x.MeasuredAtUtc, x => x);
            }
        }

        public Dictionary<string, NutritionRecord> GetExistingNutritionRecords(string source, IEnumerable<string> healthConnectRecordIds)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctHealthConnectRecordIds = healthConnectRecordIds
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                return context.NutritionRecords
                    .Include(x => x.Nutrients)
                    .Where(x => x.Source == source && distinctHealthConnectRecordIds.Contains(x.HealthConnectRecordId))
                    .ToDictionary(x => x.HealthConnectRecordId, x => x);
            }
        }
    }
}