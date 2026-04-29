using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Read.Mappers
{
    public static class HeightMeasurementReadMapper
    {
        public static HeightMeasurementReadDto MapToReadDto(HeightMeasurement heightMeasurement)
        {
            return new HeightMeasurementReadDto
            {
                HeightCm = heightMeasurement.HeightCm,
                MeasuredAtUtc = heightMeasurement.MeasuredAtUtc
            };
        }

        public static List<HeightMeasurementReadDto> MapToReadDtos(List<HeightMeasurement> heightMeasurements)
        {
            return heightMeasurements
                .Select(MapToReadDto)
                .ToList();
        }
    }
}