using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Read.Mappers;

namespace Logic.Read
{
    public class WeightMeasurementReadLogic : IWeightMeasurementReadLogic
    {
        private readonly IWeightMeasurementDataAccess _weightMeasurementDataAccess;

        public WeightMeasurementReadLogic(IWeightMeasurementDataAccess weightMeasurementDataAccess)
        {
            _weightMeasurementDataAccess = weightMeasurementDataAccess;
        }

        public List<WeightMeasurementReadDto> GetWeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc)
        {
            var weightMeasurements = _weightMeasurementDataAccess.GetWeightMeasurements(fromMeasuredAtUtc, toMeasuredAtUtc);

            return WeightMeasurementReadMapper.MapToReadDtos(weightMeasurements);
        }
    }
}