using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Read.Mappers;

namespace Logic.Read
{
    public class SleepSessionReadLogic : ISleepSessionReadLogic
    {
        private readonly ISleepSessionDataAccess _sleepSessionDataAccess;

        public SleepSessionReadLogic(ISleepSessionDataAccess sleepSessionDataAccess)
        {
            _sleepSessionDataAccess = sleepSessionDataAccess;
        }

        public List<SleepSessionReadDto> GetSleepSessions(DateOnly? from, DateOnly? to)
        {
            var sleepSessions = _sleepSessionDataAccess.GetSleepSessions(from, to);

            return SleepSessionReadMapper.MapToReadDtos(sleepSessions);
        }
    }
}