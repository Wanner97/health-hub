using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Mappers
{
    public static class ActivityImportMapper
    {
        public static List<ActivityDay> MapToActivityDays(ActivityExportDto dto)
        {
            return dto.Records.Select(x => new ActivityDay
            {
                Source = dto.Source,
                Date = x.Date,
                StartTimeUtc = x.StartTime.UtcDateTime,
                EndTimeUtc = x.EndTime.UtcDateTime,
                Steps = x.Steps,
                DistanceMeters = x.DistanceMeters
            }).ToList();
        }

        public static ImportBatch MapToImportBatch(
            ActivityExportDto dto,
            DateTime importedAtUtc,
            List<ActivityDay> activityDays)
        {
            return new ImportBatch
            {
                Source = dto.Source,
                ExportVersion = dto.ExportVersion,
                ExportedAtUtc = dto.ExportedAt.UtcDateTime,
                ImportedAtUtc = importedAtUtc,
                RangeStartUtc = dto.RangeStart.UtcDateTime,
                RangeEndUtc = dto.RangeEnd.UtcDateTime,
                ReceivedRecordCount = activityDays.Count,
                InsertedRecordCount = 0,
                UpdatedRecordCount = 0,
                UnchangedRecordCount = 0,
                ActivityDayEntries = activityDays
            };
        }
    }
}