using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Import.Mappers
{
    public static class ActivityImportMapper
    {
        public static List<ActivityDay> MapToActivityDays(string source, ActivityClusterDto? activityCluster)
        {
            if (activityCluster?.Records == null || activityCluster.Records.Count == 0)
            {
                return new List<ActivityDay>();
            }

            return activityCluster.Records.Select(x => new ActivityDay
            {
                Source = source,
                Date = x.Date,
                StartTimeUtc = x.StartTime.UtcDateTime,
                EndTimeUtc = x.EndTime.UtcDateTime,
                Steps = x.Steps,
                DistanceMeters = x.DistanceMeters
            }).ToList();
        }
    }
}