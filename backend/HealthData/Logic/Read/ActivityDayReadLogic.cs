using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Read.Mappers;

namespace Logic.Read
{
    public class ActivityDayReadLogic : IActivityDayReadLogic
    {
        private readonly IActivityDayDataAccess _activityDayDataAccess;

        public ActivityDayReadLogic(IActivityDayDataAccess activityDayDataAccess)
        {
            _activityDayDataAccess = activityDayDataAccess;
        }

        public List<ActivityDayReadDto> GetActivityDays(DateOnly? from, DateOnly? to)
        {
            var activityDays = _activityDayDataAccess.GetActivityDays(from, to);

            return ActivityDayReadMapper.MapToReadDtos(activityDays);
        }
    }
}