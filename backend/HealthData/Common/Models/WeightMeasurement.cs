namespace Common.Models
{
    public class WeightMeasurement : IImportTrackedEntity
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public double WeightKg { get; set; }

        public DateTime MeasuredAtUtc { get; set; }

        public DateTime LastImportedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;
    }
}