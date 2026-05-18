namespace Common.Models
{
    public class NutritionDay
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public int RecordCount { get; set; }

        public DateTime LastCalculatedAtUtc { get; set; }

        public int LastImportBatchId { get; set; }

        public ImportBatch LastImportBatch { get; set; } = null!;

        public List<NutritionRecord> Records { get; set; } = new();

        public List<NutritionDayNutrientTotal> NutrientTotals { get; set; } = new();

        public List<NutritionMealTypeSummary> MealTypeSummaries { get; set; } = new();
    }
}