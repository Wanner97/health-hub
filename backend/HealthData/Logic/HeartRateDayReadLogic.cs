using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Mappers;

namespace Logic
{
    public class HeartRateDayReadLogic : IHeartRateDayReadLogic
    {
        private readonly IHeartRateDayDataAccess _heartRateDayDataAccess;

        public HeartRateDayReadLogic(IHeartRateDayDataAccess heartRateDayDataAccess)
        {
            _heartRateDayDataAccess = heartRateDayDataAccess;
        }

        public List<HeartRateDayReadDto> GetHeartRateDays(DateOnly? from, DateOnly? to, bool includeHourlyRecords)
        {
            var heartRateDays = _heartRateDayDataAccess.GetHeartRateDays(from, to, includeHourlyRecords);

            return HeartRateDayReadMapper.MapToReadDtos(heartRateDays, includeHourlyRecords);
        }
    }
}