namespace Common.Models
{
    public class NutritionMealTypeNutrientTotal
    {
        public int Id { get; set; }

        public int NutritionMealTypeSummaryId { get; set; }

        public NutritionMealTypeSummary NutritionMealTypeSummary { get; set; } = null!;

        public string NutrientKey { get; set; } = string.Empty;

        public double TotalAmount { get; set; }

        public string Unit { get; set; } = string.Empty;
    }
}