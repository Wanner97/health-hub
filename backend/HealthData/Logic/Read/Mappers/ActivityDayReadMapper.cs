using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Read.Mappers
{
    public static class ActivityDayReadMapper
    {
        public static ActivityDayReadDto MapToReadDto(ActivityDay activityDay)
        {
            return new ActivityDayReadDto
            {
                Date = activityDay.Date,
                Steps = activityDay.Steps,
                DistanceMeters = activityDay.DistanceMeters,
                StartTimeUtc = activityDay.StartTimeUtc,
                EndTimeUtc = activityDay.EndTimeUtc
            };
        }

        public static List<ActivityDayReadDto> MapToReadDtos(List<ActivityDay> activityDays)
        {
            return activityDays.Select(MapToReadDto).ToList();
        }
    }
}