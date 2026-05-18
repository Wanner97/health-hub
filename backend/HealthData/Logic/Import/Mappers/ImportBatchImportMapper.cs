using Common.Dtos.DataImportDtos;
using Common.Models;
using Logic.Import.Helpers;

namespace Logic.Import.Mappers
{
    public static class ImportBatchImportMapper
    {
        public static ImportBatch MapToImportBatch(
            HealthExportDto dto,
            DateTime importedAtUtc,
            List<ActivityDay> activityDays,
            List<SleepSession> sleepSessions,
            List<HeartRateDay> heartRateDays,
            List<BloodOxygenDay> bloodOxygenDays,
            List<HeightMeasurement> heightMeasurements,
            List<WeightMeasurement> weightMeasurements,
            List<NutritionRecord> nutritionRecords)
        {
            var importBatch = new ImportBatch
            {
                Source = dto.Source,
                ExportVersion = dto.ExportVersion,
                ExportType = dto.ExportType,
                ExportedAtUtc = dto.ExportedAt.UtcDateTime,
                ImportedAtUtc = importedAtUtc,
                RangeStartUtc = dto.RangeStart.UtcDateTime,
                RangeEndUtc = dto.RangeEnd.UtcDateTime,
                ReceivedRecordCount = 0,
                InsertedRecordCount = 0,
                UpdatedRecordCount = 0,
                UnchangedRecordCount = 0,
                ActivityDayEntries = activityDays,
                SleepSessionEntries = sleepSessions,
                HeartRateDayEntries = heartRateDays,
                BloodOxygenDayEntries = bloodOxygenDays,
                HeightMeasurementEntries = heightMeasurements,
                WeightMeasurementEntries = weightMeasurements,
                NutritionRecordEntries = nutritionRecords
            };

            importBatch.ReceivedRecordCount = ImportBatchRecordCountHelper.CalculateReceivedRecordCount(importBatch);

            return importBatch;
        }
    }
}