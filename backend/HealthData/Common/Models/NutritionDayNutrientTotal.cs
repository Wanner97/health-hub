namespace Common.Models
{
    public class NutritionDayNutrientTotal
    {
        public int Id { get; set; }

        public int NutritionDayId { get; set; }

        public NutritionDay NutritionDay { get; set; } = null!;

        public string NutrientKey { get; set; } = string.Empty;

        public double TotalAmount { get; set; }

        public string Unit { get; set; } = string.Empty;
    }
}