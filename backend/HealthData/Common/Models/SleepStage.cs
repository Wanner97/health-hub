namespace Common.Models
{
    public class SleepStage
    {
        public int Id { get; set; }

        public int SleepSessionId { get; set; }

        public SleepSession SleepSession { get; set; } = null!;

        public string Stage { get; set; } = string.Empty;

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }
    }
}