using Common.Dtos.DataReadDtos;

namespace Logic.Interfaces
{
    public interface IWeightMeasurementReadLogic
    {
        List<WeightMeasurementReadDto> GetWeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc);
    }
}