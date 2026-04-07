namespace Common.Models
{
    public class ActivityDay
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int Steps { get; set; }

        public double DistanceMeters { get; set; }

        public DateTime LastImportedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;
    }
}
