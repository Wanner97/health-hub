namespace Common.Dtos.DataReadDtos
{
    public class NutritionDayReadDto
    {
        public DateOnly Date { get; set; }

        public int RecordCount { get; set; }

        public double TotalEnergyKcal { get; set; }

        public double TotalCarbohydrateGrams { get; set; }

        public double TotalFatGrams { get; set; }

        public double TotalProteinGrams { get; set; }

        public double CarbohydratePercent { get; set; }

        public double FatPercent { get; set; }

        public double ProteinPercent { get; set; }

        public List<NutritionRecordReadDto> Records { get; set; } = new();
    }
}