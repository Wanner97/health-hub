using Common.Dtos.DataReadDtos;

namespace Logic.Interfaces
{
    public interface IActivityDayReadLogic
    {
        List<ActivityDayReadDto> GetActivityDays(DateOnly? from, DateOnly? to);
    }
}