using Common.Dtos.DataReadDtos;

namespace Logic.Interfaces
{
    public interface IHeightMeasurementReadLogic
    {
        List<HeightMeasurementReadDto> GetHeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc);
    }
}