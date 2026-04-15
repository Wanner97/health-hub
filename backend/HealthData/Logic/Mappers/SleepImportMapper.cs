using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Mappers
{
    public static class SleepImportMapper
    {
        public static List<SleepSession> MapToSleepSessions(
            string source,
            DateTime importedAtUtc,
            List<SleepSessionDto> sleepSessionDtos)
        {
            return sleepSessionDtos.Select(x => new SleepSession
            {
                Source = source,
                StartTimeUtc = x.StartTime.UtcDateTime,
                EndTimeUtc = x.EndTime.UtcDateTime,
                DurationMinutes = x.DurationMinutes,
                LastImportedAtUtc = importedAtUtc,
                SleepStages = x.Stages.Select(stage => new SleepStage
                {
                    Stage = stage.Stage,
                    StartTimeUtc = stage.StartTime.UtcDateTime,
                    EndTimeUtc = stage.EndTime.UtcDateTime
                }).ToList()
            }).ToList();
        }
    }
}