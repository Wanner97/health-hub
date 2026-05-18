namespace Common.Dtos.DataReadDtos
{
    public class NutritionRecordReadDto
    {
        public string HealthConnectRecordId { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public string MealType { get; set; } = string.Empty;

        public string? Name { get; set; }

        public double TotalEnergyKcal { get; set; }

        public double TotalCarbohydrateGrams { get; set; }

        public double TotalFatGrams { get; set; }

        public double TotalProteinGrams { get; set; }

        public List<NutritionNutrientReadDto> Nutrients { get; set; } = new();
    }
}