using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Mappers
{
    public static class ImportBatchImportMapper
    {
        public static ImportBatch MapToImportBatch(
            HealthExportDto dto,
            DateTime importedAtUtc,
            List<ActivityDay> activityDays,
            List<SleepSession> sleepSessions,
            List<HeartRateDay> heartRateDays)
        {
            return new ImportBatch
            {
                Source = dto.Source,
                ExportVersion = dto.ExportVersion,
                ExportType = dto.ExportType,
                ExportedAtUtc = dto.ExportedAt.UtcDateTime,
                ImportedAtUtc = importedAtUtc,
                RangeStartUtc = dto.RangeStart.UtcDateTime,
                RangeEndUtc = dto.RangeEnd.UtcDateTime,
                ReceivedRecordCount =
                    activityDays.Count
                    + sleepSessions.Count
                    + heartRateDays.Count
                    + heartRateDays.Sum(x => x.HourlyRecords.Count),
                InsertedRecordCount = 0,
                UpdatedRecordCount = 0,
                UnchangedRecordCount = 0,
                ActivityDayEntries = activityDays,
                SleepSessionEntries = sleepSessions,
                HeartRateDayEntries = heartRateDays
            };
        }
    }
}