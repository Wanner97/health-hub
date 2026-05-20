using Common.Import;
using Common.Models;
using DataAccess.Context;
using DataAccess.Import;
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
                var query = context.ImportBatches.AsNoTracking().AsQueryable();

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

        public ImportBatch ApplyImport(ImportBatch importBatch, HealthImportUpsertData upsertData)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.ImportBatches.Add(importBatch);
                    context.SaveChanges();

                    SetImportMetadataForAllItems(upsertData, importBatch);

                    ApplyActivityDayUpserts(context, upsertData.ActivityDays);
                    ApplySleepSessionUpserts(context, upsertData.SleepSessions);
                    ApplyHeartRateDayUpserts(context, upsertData.HeartRateDays);
                    ApplyBloodOxygenDayUpserts(context, upsertData.BloodOxygenDays);
                    ApplyHeightMeasurementUpserts(context, upsertData.HeightMeasurements);
                    ApplyWeightMeasurementUpserts(context, upsertData.WeightMeasurements);

                    var originalUpdatedNutritionRecordDates = ApplyNutritionRecordUpserts(context, upsertData.NutritionRecords);

                    var affectedNutritionDates = NutritionDayAggregationHelper.GetAffectedNutritionDates(
                        upsertData.NutritionRecords.InsertedItems,
                        upsertData.NutritionRecords.UpdatedItems,
                        originalUpdatedNutritionRecordDates);

                    context.SaveChanges();

                    NutritionDayAggregationHelper.RecalculateNutritionDays(
                        context,
                        importBatch,
                        affectedNutritionDates);

                    context.SaveChanges();

                    transaction.Commit();

                    return importBatch;
                }
            }
        }

        private static void SetImportMetadataForAllItems(HealthImportUpsertData upsertData, ImportBatch importBatch)
        {
            SetImportMetadata(upsertData.ActivityDays.InsertedItems, importBatch);
            SetImportMetadata(upsertData.ActivityDays.UpdatedItems, importBatch);

            SetImportMetadata(upsertData.SleepSessions.InsertedItems, importBatch);
            SetImportMetadata(upsertData.SleepSessions.UpdatedItems, importBatch);

            SetImportMetadata(upsertData.HeartRateDays.InsertedItems, importBatch);
            SetImportMetadata(upsertData.HeartRateDays.UpdatedItems, importBatch);

            SetImportMetadata(upsertData.BloodOxygenDays.InsertedItems, importBatch);
            SetImportMetadata(upsertData.BloodOxygenDays.UpdatedItems, importBatch);

            SetImportMetadata(upsertData.HeightMeasurements.InsertedItems, importBatch);
            SetImportMetadata(upsertData.HeightMeasurements.UpdatedItems, importBatch);

            SetImportMetadata(upsertData.WeightMeasurements.InsertedItems, importBatch);
            SetImportMetadata(upsertData.WeightMeasurements.UpdatedItems, importBatch);

            SetImportMetadata(upsertData.NutritionRecords.InsertedItems, importBatch);
            SetImportMetadata(upsertData.NutritionRecords.UpdatedItems, importBatch);
        }

        private static void ApplyActivityDayUpserts(AppDbContext context, ImportUpsertData<ActivityDay> upsertData)
        {
            if (upsertData.InsertedItems.Count > 0)
            {
                context.ActivityDays.AddRange(upsertData.InsertedItems);
            }

            if (upsertData.UpdatedItems.Count > 0)
            {
                context.ActivityDays.UpdateRange(upsertData.UpdatedItems);
            }
        }

        private static void ApplySleepSessionUpserts(AppDbContext context, ImportUpsertData<SleepSession> upsertData)
        {
            if (upsertData.InsertedItems.Count > 0)
            {
                context.SleepSessions.AddRange(upsertData.InsertedItems);
            }

            if (upsertData.UpdatedItems.Count == 0)
            {
                return;
            }

            var updatedSleepSessionIds = upsertData.UpdatedItems
                .Select(x => x.Id)
                .ToList();

            var existingSleepStages = context.SleepStages
                .Where(x => updatedSleepSessionIds.Contains(x.SleepSessionId));

            context.SleepStages.RemoveRange(existingSleepStages);

            var replacementSleepStages = new List<SleepStage>();

            foreach (var updatedSleepSession in upsertData.UpdatedItems)
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

        private static void ApplyHeartRateDayUpserts(AppDbContext context, ImportUpsertData<HeartRateDay> upsertData)
        {
            if (upsertData.InsertedItems.Count > 0)
            {
                context.HeartRateDays.AddRange(upsertData.InsertedItems);
            }

            if (upsertData.UpdatedItems.Count == 0)
            {
                return;
            }

            var updatedHeartRateDayIds = upsertData.UpdatedItems
                .Select(x => x.Id)
                .ToList();

            var existingHourlyRecords = context.HeartRateHourlyRecords
                .Where(x => updatedHeartRateDayIds.Contains(x.HeartRateDayId));

            context.HeartRateHourlyRecords.RemoveRange(existingHourlyRecords);

            var replacementHourlyRecords = new List<HeartRateHourlyRecord>();

            foreach (var updatedHeartRateDay in upsertData.UpdatedItems)
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

        private static void ApplyBloodOxygenDayUpserts(AppDbContext context, ImportUpsertData<BloodOxygenDay> upsertData)
        {
            if (upsertData.InsertedItems.Count > 0)
            {
                context.BloodOxygenDays.AddRange(upsertData.InsertedItems);
            }

            if (upsertData.UpdatedItems.Count > 0)
            {
                context.BloodOxygenDays.UpdateRange(upsertData.UpdatedItems);
            }
        }

        private static void ApplyHeightMeasurementUpserts(AppDbContext context, ImportUpsertData<HeightMeasurement> upsertData)
        {
            if (upsertData.InsertedItems.Count > 0)
            {
                context.HeightMeasurements.AddRange(upsertData.InsertedItems);
            }

            if (upsertData.UpdatedItems.Count > 0)
            {
                context.HeightMeasurements.UpdateRange(upsertData.UpdatedItems);
            }
        }

        private static void ApplyWeightMeasurementUpserts(AppDbContext context, ImportUpsertData<WeightMeasurement> upsertData)
        {
            if (upsertData.InsertedItems.Count > 0)
            {
                context.WeightMeasurements.AddRange(upsertData.InsertedItems);
            }

            if (upsertData.UpdatedItems.Count > 0)
            {
                context.WeightMeasurements.UpdateRange(upsertData.UpdatedItems);
            }
        }

        private static List<DateOnly> ApplyNutritionRecordUpserts(AppDbContext context, ImportUpsertData<NutritionRecord> upsertData)
        {
            if (upsertData.InsertedItems.Count > 0)
            {
                context.NutritionRecords.AddRange(upsertData.InsertedItems);
            }

            var originalUpdatedNutritionRecordDates = new List<DateOnly>();

            if (upsertData.UpdatedItems.Count == 0)
            {
                return originalUpdatedNutritionRecordDates;
            }

            var updatedNutritionRecordIds = upsertData.UpdatedItems
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

            foreach (var updatedNutritionRecord in upsertData.UpdatedItems)
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

            return originalUpdatedNutritionRecordDates;
        }

        private static void SetImportMetadata<T>(IEnumerable<T> items, ImportBatch importBatch)
            where T : IImportTrackedEntity
        {
            foreach (var item in items)
            {
                item.LastImportBatchId = importBatch.Id;
                item.LastImportBatch = importBatch;
                item.LastImportedAtUtc = importBatch.ImportedAtUtc;
            }
        }
    }
}