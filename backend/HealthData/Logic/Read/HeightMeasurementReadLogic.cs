using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Read.Mappers;

namespace Logic.Read
{
    public class HeightMeasurementReadLogic : IHeightMeasurementReadLogic
    {
        private readonly IHeightMeasurementDataAccess _heightMeasurementDataAccess;

        public HeightMeasurementReadLogic(IHeightMeasurementDataAccess heightMeasurementDataAccess)
        {
            _heightMeasurementDataAccess = heightMeasurementDataAccess;
        }

        public List<HeightMeasurementReadDto> GetHeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc)
        {
            var heightMeasurements = _heightMeasurementDataAccess.GetHeightMeasurements(fromMeasuredAtUtc, toMeasuredAtUtc);

            return HeightMeasurementReadMapper.MapToReadDtos(heightMeasurements);
        }
    }
}