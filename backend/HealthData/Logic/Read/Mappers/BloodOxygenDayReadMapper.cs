using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Read.Mappers
{
    public static class BloodOxygenDayReadMapper
    {
        public static BloodOxygenDayReadDto MapToReadDto(BloodOxygenDay bloodOxygenDay)
        {
            return new BloodOxygenDayReadDto
            {
                Date = bloodOxygenDay.Date,
                StartTimeUtc = bloodOxygenDay.StartTimeUtc,
                EndTimeUtc = bloodOxygenDay.EndTimeUtc,
                AvgPercent = bloodOxygenDay.AvgPercent,
                MinPercent = bloodOxygenDay.MinPercent,
                MaxPercent = bloodOxygenDay.MaxPercent,
                MeasurementCount = bloodOxygenDay.MeasurementCount
            };
        }

        public static List<BloodOxygenDayReadDto> MapToReadDtos(List<BloodOxygenDay> bloodOxygenDays)
        {
            return bloodOxygenDays
                .Select(MapToReadDto)
                .ToList();
        }
    }
}