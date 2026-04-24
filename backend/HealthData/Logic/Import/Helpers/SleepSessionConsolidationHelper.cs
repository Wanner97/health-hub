using Common.Dtos.DataImportDtos;

namespace Logic.Import.Helpers
{
    public static class SleepSessionConsolidationHelper
    {
        public static List<SleepSessionDto> ConsolidateSleepSessions(List<SleepSessionDto> sleepSessionDtos)
        {
            return sleepSessionDtos
                .GroupBy(x => x.StartTime.UtcDateTime)
                .Select(group => group
                    .OrderByDescending(x => x.EndTime.UtcDateTime)
                    .ThenByDescending(x => x.Stages.Count)
                    .ThenByDescending(x => x.DurationMinutes)
                    .First())
                .OrderBy(x => x.StartTime.UtcDateTime)
                .ToList();
        }
    }
}