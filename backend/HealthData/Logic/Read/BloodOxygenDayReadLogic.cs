using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Read.Mappers;

namespace Logic.Read
{
    public class BloodOxygenDayReadLogic : IBloodOxygenDayReadLogic
    {
        private readonly IBloodOxygenDayDataAccess _bloodOxygenDayDataAccess;

        public BloodOxygenDayReadLogic(IBloodOxygenDayDataAccess bloodOxygenDayDataAccess)
        {
            _bloodOxygenDayDataAccess = bloodOxygenDayDataAccess;
        }

        public List<BloodOxygenDayReadDto> GetBloodOxygenDays(DateOnly? from, DateOnly? to)
        {
            var bloodOxygenDays = _bloodOxygenDayDataAccess.GetBloodOxygenDays(from, to);

            return BloodOxygenDayReadMapper.MapToReadDtos(bloodOxygenDays);
        }
    }
}