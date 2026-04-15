namespace Common.Dtos.DataImportDtos
{
    public class SleepSessionDto
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public int DurationMinutes { get; set; }

        public List<SleepStageDto> Stages { get; set; } = new();
    }
}