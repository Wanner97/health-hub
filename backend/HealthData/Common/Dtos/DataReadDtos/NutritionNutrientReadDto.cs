namespace Common.Dtos.DataReadDtos
{
    public class NutritionNutrientReadDto
    {
        public string NutrientKey { get; set; } = string.Empty;

        public double Amount { get; set; }

        public string Unit { get; set; } = string.Empty;
    }
}