namespace Common.Dtos.DataReadDtos
{
    public class SleepSessionReadDto
    {
        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int DurationMinutes { get; set; }

        public List<SleepStageReadDto> SleepStages { get; set; } = new();
    }
}