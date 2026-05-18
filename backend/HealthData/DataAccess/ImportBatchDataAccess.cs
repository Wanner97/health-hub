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

        public Dictionary<DateTime, HeightMeasurement> GetExistingHeightMeasurements(
            string source,
            IEnumerable<DateTime> measuredAtUtcValues)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctMeasuredAtUtcValues = measuredAtUtcValues.Distinct().ToList();

                return context.HeightMeasurements
                    .Where(x => x.Source == source && distinctMeasuredAtUtcValues.Contains(x.MeasuredAtUtc))
                    .ToDictionary(x => x.MeasuredAtUtc, x => x);
            }
        }

        public Dictionary<DateTime, WeightMeasurement> GetExistingWeightMeasurements(string source, IEnumerable<DateTime> measuredAtUtcValues)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var distinctMeasuredAtUtcValues = measuredAtUtcValues.Distinct().ToList();

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

                    var insertedHeightMeasurements = upsertData.HeightMeasurements.InsertedItems;
                    var updatedHeightMeasurements = upsertData.HeightMeasurements.UpdatedItems;

                    var insertedWeightMeasurements = upsertData.WeightMeasurements.InsertedItems;
                    var updatedWeightMeasurements = upsertData.WeightMeasurements.UpdatedItems;

                    var insertedNutritionRecords = upsertData.NutritionRecords.InsertedItems;
                    var updatedNutritionRecords = upsertData.NutritionRecords.UpdatedItems;

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

                    foreach (var insertedHeightMeasurement in insertedHeightMeasurements)
                    {
                        insertedHeightMeasurement.LastImportBatchId = importBatch.Id;
                        insertedHeightMeasurement.LastImportBatch = importBatch;
                        insertedHeightMeasurement.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var updatedHeightMeasurement in updatedHeightMeasurements)
                    {
                        updatedHeightMeasurement.LastImportBatchId = importBatch.Id;
                        updatedHeightMeasurement.LastImportBatch = importBatch;
                        updatedHeightMeasurement.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var insertedWeightMeasurement in insertedWeightMeasurements)
                    {
                        insertedWeightMeasurement.LastImportBatchId = importBatch.Id;
                        insertedWeightMeasurement.LastImportBatch = importBatch;
                        insertedWeightMeasurement.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var updatedWeightMeasurement in updatedWeightMeasurements)
                    {
                        updatedWeightMeasurement.LastImportBatchId = importBatch.Id;
                        updatedWeightMeasurement.LastImportBatch = importBatch;
                        updatedWeightMeasurement.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var insertedNutritionRecord in insertedNutritionRecords)
                    {
                        insertedNutritionRecord.LastImportBatchId = importBatch.Id;
                        insertedNutritionRecord.LastImportBatch = importBatch;
                        insertedNutritionRecord.LastImportedAtUtc = importBatch.ImportedAtUtc;
                    }

                    foreach (var updatedNutritionRecord in updatedNutritionRecords)
                    {
                        updatedNutritionRecord.LastImportBatchId = importBatch.Id;
                        updatedNutritionRecord.LastImportBatch = importBatch;
                        updatedNutritionRecord.LastImportedAtUtc = importBatch.ImportedAtUtc;
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

                    if (insertedHeightMeasurements.Count > 0)
                    {
                        context.HeightMeasurements.AddRange(insertedHeightMeasurements);
                    }

                    if (updatedHeightMeasurements.Count > 0)
                    {
                        context.HeightMeasurements.UpdateRange(updatedHeightMeasurements);
                    }

                    if (insertedWeightMeasurements.Count > 0)
                    {
                        context.WeightMeasurements.AddRange(insertedWeightMeasurements);
                    }

                    if (updatedWeightMeasurements.Count > 0)
                    {
                        context.WeightMeasurements.UpdateRange(updatedWeightMeasurements);
                    }

                    if (insertedNutritionRecords.Count > 0)
                    {
                        context.NutritionRecords.AddRange(insertedNutritionRecords);
                    }

                    var originalUpdatedNutritionRecordDates = new List<DateOnly>();

                    if (updatedNutritionRecords.Count > 0)
                    {
                        var updatedNutritionRecordIds = updatedNutritionRecords
                            .Select(x => x.Id)
                            .ToList();

                        originalUpdatedNutritionRecordDates = context.NutritionRecords
                            .Where(x => updatedNutritionRecordIds.Contains(x.Id))
                            .Select(x => x.Date)
                            .ToList();

                        var existingNutritionRecordNutrients = context.NutritionRecordNutrients
                            .Where(x => updatedNutritionRecordIds.Contains(x.NutritionRecordId));

                        context.NutritionRecordNutrients.RemoveRange(existingNutritionRecordNutrients);

                        var replacementNutritionRecordNutrients = new List<NutritionRecordNutrient>();

                        foreach (var updatedNutritionRecord in updatedNutritionRecords)
                        {
                            var newNutritionRecordNutrients = updatedNutritionRecord.Nutrients.ToList();

                            updatedNutritionRecord.Nutrients = new List<NutritionRecordNutrient>();

                            context.NutritionRecords.Update(updatedNutritionRecord);

                            foreach (var newNutritionRecordNutrient in newNutritionRecordNutrients)
                            {
                                newNutritionRecordNutrient.NutritionRecordId = updatedNutritionRecord.Id;
                                replacementNutritionRecordNutrients.Add(newNutritionRecordNutrient);
                            }
                        }

                        if (replacementNutritionRecordNutrients.Count > 0)
                        {
                            context.NutritionRecordNutrients.AddRange(replacementNutritionRecordNutrients);
                        }
                    }

                    var affectedNutritionDates = GetAffectedNutritionDates(insertedNutritionRecords, updatedNutritionRecords, originalUpdatedNutritionRecordDates);

                    context.SaveChanges();

                    RecalculateNutritionDays(context, importBatch, affectedNutritionDates);

                    context.SaveChanges();

                    transaction.Commit();

                    return importBatch;
                }
            }
        }

        private static List<DateOnly> GetAffectedNutritionDates(List<NutritionRecord> insertedNutritionRecords, List<NutritionRecord> updatedNutritionRecords, List<DateOnly> originalUpdatedNutritionRecordDates)
        {
            return insertedNutritionRecords
                .Select(x => x.Date)
                .Concat(updatedNutritionRecords.Select(x => x.Date))
                .Concat(originalUpdatedNutritionRecordDates)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        private static List<NutritionDayNutrientTotal> BuildNutritionDayNutrientTotals(List<NutritionRecord> nutritionRecords)
        {
            return nutritionRecords
                .SelectMany(x => x.Nutrients)
                .GroupBy(x => x.NutrientKey)
                .Select(group => new NutritionDayNutrientTotal
                {
                    NutrientKey = group.Key,
                    TotalAmount = group.Sum(x => x.Amount),
                    Unit = group.First().Unit
                })
                .OrderBy(x => x.NutrientKey)
                .ToList();
        }

        private static List<NutritionMealTypeSummary> BuildNutritionMealTypeSummaries(List<NutritionRecord> nutritionRecords)
        {
            return nutritionRecords
                .GroupBy(x => x.MealType)
                .Select(mealTypeGroup => new NutritionMealTypeSummary
                {
                    MealType = mealTypeGroup.Key,
                    RecordCount = mealTypeGroup.Count(),
                    NutrientTotals = mealTypeGroup
                        .SelectMany(x => x.Nutrients)
                        .GroupBy(x => x.NutrientKey)
                        .Select(nutrientGroup => new NutritionMealTypeNutrientTotal
                        {
                            NutrientKey = nutrientGroup.Key,
                            TotalAmount = nutrientGroup.Sum(x => x.Amount),
                            Unit = nutrientGroup.First().Unit
                        })
                        .OrderBy(x => x.NutrientKey)
                        .ToList()
                })
                .OrderBy(x => x.MealType)
                .ToList();
        }

        private void RecalculateNutritionDays(AppDbContext context, ImportBatch importBatch, IReadOnlyCollection<DateOnly> affectedDates)
        {
            if (affectedDates.Count == 0)
            {
                return;
            }

            foreach (var affectedDate in affectedDates)
            {
                RecalculateNutritionDay(context, importBatch, affectedDate);
            }
        }

        private void RecalculateNutritionDay(AppDbContext context, ImportBatch importBatch, DateOnly affectedDate)
        {
            var nutritionRecordsForDay = context.NutritionRecords
                .Include(x => x.Nutrients)
                .Where(x => x.Source == importBatch.Source && x.Date == affectedDate)
                .OrderBy(x => x.StartTimeUtc)
                .ToList();

            var existingNutritionDay = context.NutritionDays
                .Include(x => x.NutrientTotals)
                .Include(x => x.MealTypeSummaries)
                    .ThenInclude(x => x.NutrientTotals)
                .FirstOrDefault(x => x.Source == importBatch.Source && x.Date == affectedDate);

            if (existingNutritionDay != null)
            {
                RemoveExistingNutritionDayAggregates(context, existingNutritionDay);
                context.SaveChanges();
            }

            if (nutritionRecordsForDay.Count == 0)
            {
                if (existingNutritionDay != null)
                {
                    context.NutritionDays.Remove(existingNutritionDay);
                }

                return;
            }

            var nutritionDay = existingNutritionDay ?? new NutritionDay
            {
                Source = importBatch.Source,
                Date = affectedDate
            };

            nutritionDay.RecordCount = nutritionRecordsForDay.Count;
            nutritionDay.LastCalculatedAtUtc = importBatch.ImportedAtUtc;
            nutritionDay.LastImportBatchId = importBatch.Id;
            nutritionDay.LastImportBatch = importBatch;
            nutritionDay.NutrientTotals = BuildNutritionDayNutrientTotals(nutritionRecordsForDay);
            nutritionDay.MealTypeSummaries = BuildNutritionMealTypeSummaries(nutritionRecordsForDay);

            foreach (var nutritionRecord in nutritionRecordsForDay)
            {
                nutritionRecord.NutritionDay = nutritionDay;
            }

            if (existingNutritionDay == null)
            {
                context.NutritionDays.Add(nutritionDay);
            }
            else
            {
                context.NutritionDays.Update(nutritionDay);
            }
        }

        private static void RemoveExistingNutritionDayAggregates(AppDbContext context, NutritionDay existingNutritionDay)
        {
            var existingMealTypeNutrientTotals = existingNutritionDay.MealTypeSummaries
                .SelectMany(x => x.NutrientTotals)
                .ToList();

            if (existingMealTypeNutrientTotals.Count > 0)
            {
                context.NutritionMealTypeNutrientTotals.RemoveRange(existingMealTypeNutrientTotals);
            }

            if (existingNutritionDay.MealTypeSummaries.Count > 0)
            {
                context.NutritionMealTypeSummaries.RemoveRange(existingNutritionDay.MealTypeSummaries);
            }

            if (existingNutritionDay.NutrientTotals.Count > 0)
            {
                context.NutritionDayNutrientTotals.RemoveRange(existingNutritionDay.NutrientTotals);
            }

            existingNutritionDay.MealTypeSummaries = new List<NutritionMealTypeSummary>();
            existingNutritionDay.NutrientTotals = new List<NutritionDayNutrientTotal>();
        }
    }
}