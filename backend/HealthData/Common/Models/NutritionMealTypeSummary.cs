 namespace Common.Models
{
    public class NutritionMealTypeSummary
    {
        public int Id { get; set; }

        public int NutritionDayId { get; set; }

        public NutritionDay NutritionDay { get; set; } = null!;

        public string MealType { get; set; } = string.Empty;

        public int RecordCount { get; set; }

        public List<NutritionMealTypeNutrientTotal> NutrientTotals { get; set; } = new();
    }
}