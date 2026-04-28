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
                StartTimeUtc = activityDay.StartTimeUtc,
                EndTimeUtc = activityDay.EndTimeUtc,
                Steps = activityDay.Steps,
                DistanceMeters = activityDay.DistanceMeters,
                DistanceSource = activityDay.DistanceSource,
                TotalCaloriesBurnedKcal = activityDay.TotalCaloriesBurnedKcal
            };
        }

        public static List<ActivityDayReadDto> MapToReadDtos(List<ActivityDay> activityDays)
        {
            return activityDays
                .Select(MapToReadDto)
                .ToList();
        }
    }
}