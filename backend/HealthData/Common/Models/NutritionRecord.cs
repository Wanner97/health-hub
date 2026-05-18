namespace Common.Models
{
    public class NutritionRecord
    {
        public int Id { get; set; }

        public int? NutritionDayId { get; set; }

        public NutritionDay? NutritionDay { get; set; }

        public string Source { get; set; } = string.Empty;

        public string HealthConnectRecordId { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public string MealType { get; set; } = string.Empty;

        public string? Name { get; set; }

        public DateTime LastImportedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;

        public List<NutritionRecordNutrient> Nutrients { get; set; } = new();
    }
}