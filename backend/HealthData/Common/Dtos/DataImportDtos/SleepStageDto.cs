namespace Common.Dtos.DataImportDtos
{
    public class SleepStageDto
    {
        public string Stage { get; set; } = string.Empty;

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}