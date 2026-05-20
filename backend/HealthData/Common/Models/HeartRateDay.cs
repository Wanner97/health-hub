namespace Common.Models
{
    public class HeartRateDay : IImportTrackedEntity
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int AvgBpm { get; set; }

        public int MinBpm { get; set; }

        public int MaxBpm { get; set; }

        public int MeasurementCount { get; set; }

        public DateTime LastImportedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;

        public List<HeartRateHourlyRecord> HourlyRecords { get; set; } = new();
    }
}