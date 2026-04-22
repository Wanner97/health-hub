using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Mappers
{
    public static class HeartRateImportMapper
    {
        public static List<HeartRateDay> MapToHeartRateDays(
            string source,
            DateTime importedAtUtc,
            HeartRateDailyClusterDto? heartRateDailyCluster,
            HeartRateHourlyClusterDto? heartRateHourlyCluster)
        {
            var heartRateDayDtos = heartRateDailyCluster?.Records ?? new List<HeartRateDayDto>();
            var heartRateHourlyRecordDtos = heartRateHourlyCluster?.Records ?? new List<HeartRateHourlyRecordDto>();

            var heartRateDays = heartRateDayDtos
                .Select(x => new HeartRateDay
                {
                    Source = source,
                    Date = x.Date,
                    StartTimeUtc = x.StartTime.UtcDateTime,
                    EndTimeUtc = x.EndTime.UtcDateTime,
                    AvgBpm = x.AvgBpm,
                    MinBpm = x.MinBpm,
                    MaxBpm = x.MaxBpm,
                    MeasurementCount = x.MeasurementCount,
                    LastImportedAtUtc = importedAtUtc,
                    HourlyRecords = new List<HeartRateHourlyRecord>()
                })
                .ToList();

            var heartRateDaysByDate = heartRateDays.ToDictionary(x => x.Date, x => x);

            foreach (var heartRateHourlyRecordDto in heartRateHourlyRecordDtos)
            {
                if (!heartRateDaysByDate.TryGetValue(heartRateHourlyRecordDto.Date, out var heartRateDay))
                {
                    throw new InvalidOperationException(
                        $"A heart rate hourly record exists for date {heartRateHourlyRecordDto.Date}, but no matching heart rate day record was found.");
                }

                heartRateDay.HourlyRecords.Add(new HeartRateHourlyRecord
                {
                    Hour = heartRateHourlyRecordDto.Hour,
                    StartTimeUtc = heartRateHourlyRecordDto.StartTime.UtcDateTime,
                    EndTimeUtc = heartRateHourlyRecordDto.EndTime.UtcDateTime,
                    AvgBpm = heartRateHourlyRecordDto.AvgBpm,
                    MinBpm = heartRateHourlyRecordDto.MinBpm,
                    MaxBpm = heartRateHourlyRecordDto.MaxBpm,
                    MeasurementCount = heartRateHourlyRecordDto.MeasurementCount
                });
            }

            foreach (var heartRateDay in heartRateDays)
            {
                heartRateDay.HourlyRecords = heartRateDay.HourlyRecords
                    .OrderBy(x => x.Hour)
                    .ToList();
            }

            return heartRateDays
                .OrderBy(x => x.Date)
                .ToList();
        }
    }
}