namespace Common.Models
{
    public class HeightMeasurement : IImportTrackedEntity
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public double HeightCm { get; set; }

        public DateTime MeasuredAtUtc { get; set; }

        public DateTime LastImportedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;
    }
}