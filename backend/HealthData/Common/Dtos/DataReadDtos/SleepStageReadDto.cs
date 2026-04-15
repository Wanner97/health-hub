namespace Common.Dtos.DataReadDtos
{
    public class SleepStageReadDto
    {
        public string Stage { get; set; } = string.Empty;

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }
    }
}