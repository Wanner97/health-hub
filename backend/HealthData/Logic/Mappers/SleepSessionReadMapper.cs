using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Mappers
{
    public static class SleepSessionReadMapper
    {
        public static SleepSessionReadDto MapToReadDto(SleepSession sleepSession)
        {
            return new SleepSessionReadDto
            {
                StartTimeUtc = sleepSession.StartTimeUtc,
                EndTimeUtc = sleepSession.EndTimeUtc,
                DurationMinutes = sleepSession.DurationMinutes,
                SleepStages = sleepSession.SleepStages
                    .OrderBy(x => x.StartTimeUtc)
                    .Select(x => new SleepStageReadDto
                    {
                        Stage = x.Stage,
                        StartTimeUtc = x.StartTimeUtc,
                        EndTimeUtc = x.EndTimeUtc
                    })
                    .ToList()
            };
        }

        public static List<SleepSessionReadDto> MapToReadDtos(List<SleepSession> sleepSessions)
        {
            return sleepSessions.Select(MapToReadDto).ToList();
        }
    }
}