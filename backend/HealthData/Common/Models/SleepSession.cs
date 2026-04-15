namespace Common.Models
{
    public class SleepSession
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime LastImportedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;

        public List<SleepStage> SleepStages { get; set; } = new();
    }
}