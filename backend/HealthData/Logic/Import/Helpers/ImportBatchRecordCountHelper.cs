using Common.Import;
using Common.Models;

namespace Logic.Import.Helpers
{
    public static class ImportBatchRecordCountHelper
    {
        public static int CalculateReceivedRecordCount(ImportBatch importBatch)
        {
            return (importBatch.ActivityDayEntries?.Count ?? 0)
                   + (importBatch.SleepSessionEntries?.Count ?? 0)
                   + (importBatch.HeartRateDayEntries?.Count ?? 0)
                   + (importBatch.HeartRateDayEntries?.Sum(x => x.HourlyRecords?.Count ?? 0) ?? 0)
                   + (importBatch.BloodOxygenDayEntries?.Count ?? 0)
                   + (importBatch.HeightMeasurementEntries?.Count ?? 0)
                   + (importBatch.WeightMeasurementEntries?.Count ?? 0)
                   + (importBatch.NutritionRecordEntries?.Count ?? 0);
        }

        public static int CalculateInsertedRecordCount(HealthImportUpsertData upsertData)
        {
            return upsertData.ActivityDays.InsertedItems.Count
                   + upsertData.SleepSessions.InsertedItems.Count
                   + upsertData.HeartRateDays.InsertedItems.Count
                   + upsertData.BloodOxygenDays.InsertedItems.Count
                   + upsertData.HeightMeasurements.InsertedItems.Count
                   + upsertData.WeightMeasurements.InsertedItems.Count
                   + upsertData.NutritionRecords.InsertedItems.Count;
        }

        public static int CalculateUpdatedRecordCount(HealthImportUpsertData upsertData)
        {
            return upsertData.ActivityDays.UpdatedItems.Count
                   + upsertData.SleepSessions.UpdatedItems.Count
                   + upsertData.HeartRateDays.UpdatedItems.Count
                   + upsertData.BloodOxygenDays.UpdatedItems.Count
                   + upsertData.HeightMeasurements.UpdatedItems.Count
                   + upsertData.WeightMeasurements.UpdatedItems.Count
                   + upsertData.NutritionRecords.UpdatedItems.Count;
        }

        public static int CalculateUnchangedRecordCount(HealthImportUpsertData upsertData)
        {
            return upsertData.ActivityDays.UnchangedCount
                   + upsertData.SleepSessions.UnchangedCount
                   + upsertData.HeartRateDays.UnchangedCount
                   + upsertData.BloodOxygenDays.UnchangedCount
                   + upsertData.HeightMeasurements.UnchangedCount
                   + upsertData.WeightMeasurements.UnchangedCount
                   + upsertData.NutritionRecords.UnchangedCount;
        }
    }
}