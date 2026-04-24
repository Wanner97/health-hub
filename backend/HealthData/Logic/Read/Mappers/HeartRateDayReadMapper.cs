using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Read.Mappers
{
    public static class HeartRateDayReadMapper
    {
        public static HeartRateDayReadDto MapToReadDto(HeartRateDay heartRateDay, bool includeHourlyRecords)
        {
            return new HeartRateDayReadDto
            {
                Date = heartRateDay.Date,
                AvgBpm = heartRateDay.AvgBpm,
                MinBpm = heartRateDay.MinBpm,
                MaxBpm = heartRateDay.MaxBpm,
                MeasurementCount = heartRateDay.MeasurementCount,
                HourlyRecords = includeHourlyRecords
                    ? heartRateDay.HourlyRecords
                        .OrderBy(x => x.Hour)
                        .Select(x => new HeartRateHourlyRecordReadDto
                        {
                            Hour = x.Hour,
                            StartTimeUtc = x.StartTimeUtc,
                            EndTimeUtc = x.EndTimeUtc,
                            AvgBpm = x.AvgBpm,
                            MinBpm = x.MinBpm,
                            MaxBpm = x.MaxBpm,
                            MeasurementCount = x.MeasurementCount
                        })
                        .ToList()
                    : new List<HeartRateHourlyRecordReadDto>()
            };
        }

        public static List<HeartRateDayReadDto> MapToReadDtos(List<HeartRateDay> heartRateDays, bool includeHourlyRecords)
        {
            return heartRateDays
                .Select(x => MapToReadDto(x, includeHourlyRecords))
                .ToList();
        }
    }
}