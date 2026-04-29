using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Read.Mappers
{
    public static class WeightMeasurementReadMapper
    {
        public static WeightMeasurementReadDto MapToReadDto(WeightMeasurement weightMeasurement)
        {
            return new WeightMeasurementReadDto
            {
                Date = weightMeasurement.Date,
                WeightKg = weightMeasurement.WeightKg,
                MeasuredAtUtc = weightMeasurement.MeasuredAtUtc
            };
        }

        public static List<WeightMeasurementReadDto> MapToReadDtos(List<WeightMeasurement> weightMeasurements)
        {
            return weightMeasurements
                .Select(MapToReadDto)
                .ToList();
        }
    }
}