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

        public ImportBatch ApplyImport(
            ImportBatch importBatch,
            List<ActivityDay> insertedActivityDays,
            List<ActivityDay> updatedActivityDays,
            List<SleepSession> insertedSleepSessions,
            List<SleepSession> updatedSleepSessions)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
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

                    context.SaveChanges();
                    transaction.Commit();

                    return importBatch;
                }
            }
        }
    }
}