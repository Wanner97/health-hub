using Common.Dtos.DataReadDtos;

namespace Logic.Interfaces
{
    public interface ISleepSessionReadLogic
    {
        List<SleepSessionReadDto> GetSleepSessions(DateOnly? from, DateOnly? to);
    }
}