using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;

namespace Logic
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

            return activityDays.Select(x => new ActivityDayReadDto
            {
                Date = x.Date,
                Steps = x.Steps,
                DistanceMeters = x.DistanceMeters,
                StartTimeUtc = x.StartTimeUtc,
                EndTimeUtc = x.EndTimeUtc
            }).ToList();
        }
    }
}