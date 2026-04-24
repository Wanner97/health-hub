namespace Common.Models
{
    public class BloodOxygenDay
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public double AvgPercent { get; set; }

        public double MinPercent { get; set; }

        public double MaxPercent { get; set; }

        public int MeasurementCount { get; set; }

        public DateTime LastImportedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;
    }
}