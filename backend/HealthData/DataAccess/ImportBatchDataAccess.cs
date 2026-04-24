using Common.Import;
using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ImportBatchDataAccess : IImportBatchDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public ImportBatchDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public ImportBatch? GetLatestImportBatch()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.ImportBatches
                    .OrderByDescending(x => x.ImportedAtUtc)
                    .FirstOrDefault();
            }
        }

        public List<ImportBatch> GetImportBatches(DateOnly? from, DateOnly? to)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.ImportBatches.AsQueryable();

                if (from.HasValue)
                {
                    var fromDateTimeUtc = from.Value.ToDateTime(TimeOnly.MinValue);
                    query = query.Where(x => x.ImportedAtUtc >= fromDateTimeUtc);
                }

                if (to.HasValue)
                {
                    var toDateTimeUtc = to.Value.ToDateTime(TimeOnly.MaxValue);
                    query = query.Where(x => x.ImportedAtUtc <= toDateTimeUtc);
                }

                return query
                    .OrderByDescending(x => x.ImportedAtUtc)
                    .ToList();
            }
        }

        public Dictionary<DateOnly, ActivityDay> GetExistingActivityDays(string source, IEnumerable<DateOnly> dates)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctDates = dates.Distinct().ToList();

                return context.ActivityDays
                    .Where(x => x.Source == source && distinctDates.Contains(x.Date))
                    .ToDictionary(x => x.Date, x => x);
            }
        }

        public Dictionary<DateTime, SleepSession> GetExistingSleepSessions(string source, IEnumerable<DateTime> startTimes)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctStartTimes = startTimes.Distinct().ToList();

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
                var distinctDates = dates.Distinct().ToList();

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
                var distinctDates = dates.Distinct().ToList();

                return context.BloodOxygenDays
                    .Where(x => x.Source == source && distinctDates.Contains(x.Date))
                    .ToDictionary(x => x.Date, x => x);
            }
        }

        public ImportBatch ApplyImport(ImportBatch importBatch, HealthImportUpsertData upsertData)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var insertedActivityDays = upsertData.ActivityDays.InsertedItems;
                    var updatedActivityDays = upsertData.ActivityDays.UpdatedItems;

                    var insertedSleepSessions = upsertData.SleepSessions.InsertedItems;
                    var updatedSleepSessions = upsertData.SleepSessions.UpdatedItems;

                    var insertedHeartRateDays = upsertData.HeartRateDays.InsertedItems;
                    var updatedHeartRateDays = upsertData.HeartRateDays.UpdatedItems;

                    var insertedBloodOxygenDays = upsertData.BloodOxygenDays.InsertedItems;
                    var updatedBloodOxygenDays = upsertData.BloodOxygenDays.UpdatedItems;

                    context.ImportBatches.Add(importBatch);
                    context.SaveChanges();

                    foreach (var insertedActivityDay in insertedActivityDays)
                    {
                        insertedActivityDay.LastImportBatchId = importBatch.Id;
                        insertedActivityDay.LastImportBatch = importBatch;
                        insertedActivityDay.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var updatedActivityDay in updatedActivityDays)
                    {
                        updatedActivityDay.LastImportBatchId = importBatch.Id;
                        updatedActivityDay.LastImportBatch = importBatch;
                        updatedActivityDay.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var insertedSleepSession in insertedSleepSessions)
                    {
                        insertedSleepSession.LastImportBatchId = importBatch.Id;
                        insertedSleepSession.LastImportBatch = importBatch;
                        insertedSleepSession.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var updatedSleepSession in updatedSleepSessions)
                    {
                        updatedSleepSession.LastImportBatchId = importBatch.Id;
                        updatedSleepSession.LastImportBatch = importBatch;
                        updatedSleepSession.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var insertedHeartRateDay in insertedHeartRateDays)
                    {
                        insertedHeartRateDay.LastImportBatchId = importBatch.Id;
                        insertedHeartRateDay.LastImportBatch = importBatch;
                        insertedHeartRateDay.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var updatedHeartRateDay in updatedHeartRateDays)
                    {
                        updatedHeartRateDay.LastImportBatchId = importBatch.Id;
                        updatedHeartRateDay.LastImportBatch = importBatch;
                        updatedHeartRateDay.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var insertedBloodOxygenDay in insertedBloodOxygenDays)
                    {
                        insertedBloodOxygenDay.LastImportBatchId = importBatch.Id;
                        insertedBloodOxygenDay.LastImportBatch = importBatch;
                        insertedBloodOxygenDay.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var updatedBloodOxygenDay in updatedBloodOxygenDays)
                    {
                        updatedBloodOxygenDay.LastImportBatchId = importBatch.Id;
                        updatedBloodOxygenDay.LastImportBatch = importBatch;
                        updatedBloodOxygenDay.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    if (insertedActivityDays.Count > 0)
                    {
                        context.ActivityDays.AddRange(insertedActivityDays);
                    }

                    if (updatedActivityDays.Count > 0)
                    {
                        context.ActivityDays.UpdateRange(updatedActivityDays);
                    }

                    if (insertedSleepSessions.Count > 0)
                    {
                        context.SleepSessions.AddRange(insertedSleepSessions);
                    }

                    if (updatedSleepSessions.Count > 0)
                    {
                        var updatedSleepSessionIds = updatedSleepSessions
                            .Select(x => x.Id)
                            .ToList();

                        var existingSleepStages = context.SleepStages
                            .Where(x => updatedSleepSessionIds.Contains(x.SleepSessionId));

                        context.SleepStages.RemoveRange(existingSleepStages);

                        var replacementSleepStages = new List<SleepStage>();

                        foreach (var updatedSleepSession in updatedSleepSessions)
                        {
                            var newSleepStages = updatedSleepSession.SleepStages.ToList();

                            updatedSleepSession.SleepStages = new List<SleepStage>();

                            context.SleepSessions.Update(updatedSleepSession);

                            foreach (var newSleepStage in newSleepStages)
                            {
                                newSleepStage.SleepSessionId = updatedSleepSession.Id;
                                replacementSleepStages.Add(newSleepStage);
                            }
                        }

                        if (replacementSleepStages.Count > 0)
                        {
                            context.SleepStages.AddRange(replacementSleepStages);
                        }
                    }

                    if (insertedHeartRateDays.Count > 0)
                    {
                        context.HeartRateDays.AddRange(insertedHeartRateDays);
                    }

                    if (updatedHeartRateDays.Count > 0)
                    {
                        var updatedHeartRateDayIds = updatedHeartRateDays
                            .Select(x => x.Id)
                            .ToList();

                        var existingHourlyRecords = context.HeartRateHourlyRecords
                            .Where(x => updatedHeartRateDayIds.Contains(x.HeartRateDayId));

                        context.HeartRateHourlyRecords.RemoveRange(existingHourlyRecords);

                        var replacementHourlyRecords = new List<HeartRateHourlyRecord>();

                        foreach (var updatedHeartRateDay in updatedHeartRateDays)
                        {
                            var newHourlyRecords = updatedHeartRateDay.HourlyRecords.ToList();

                            updatedHeartRateDay.HourlyRecords = new List<HeartRateHourlyRecord>();

                            context.HeartRateDays.Update(updatedHeartRateDay);

                            foreach (var newHourlyRecord in newHourlyRecords)
                            {
                                newHourlyRecord.HeartRateDayId = updatedHeartRateDay.Id;
                                replacementHourlyRecords.Add(newHourlyRecord);
                            }
                        }

                        if (replacementHourlyRecords.Count > 0)
                        {
                            context.HeartRateHourlyRecords.AddRange(replacementHourlyRecords);
                        }
                    }

                    if (insertedBloodOxygenDays.Count > 0)
                    {
                        context.BloodOxygenDays.AddRange(insertedBloodOxygenDays);
                    }

                    if (updatedBloodOxygenDays.Count > 0)
                    {
                        context.BloodOxygenDays.UpdateRange(updatedBloodOxygenDays);
                    }

                    context.SaveChanges();
                    transaction.Commit();

                    return importBatch;
                }
            }
        }
    }
}