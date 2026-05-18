namespace Common.Models
{
    public class NutritionRecordNutrient
    {
        public int Id { get; set; }

        public int NutritionRecordId { get; set; }

        public NutritionRecord NutritionRecord { get; set; } = null!;

        public string NutrientKey { get; set; } = string.Empty;

        public double Amount { get; set; }

        public string Unit { get; set; } = string.Empty;
    }
}