namespace Common.Dtos.DataImportDtos
{
    public class NutritionRecordDto
    {
        public DateOnly Date { get; set; }

        public string HealthConnectRecordId { get; set; } = string.Empty;

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string MealType { get; set; } = string.Empty;

        public string? Name { get; set; }

        public double? EnergyKcal { get; set; }

        public double? EnergyFromFatKcal { get; set; }

        public double? BiotinGrams { get; set; }

        public double? CaffeineGrams { get; set; }

        public double? CalciumGrams { get; set; }

        public double? ChlorideGrams { get; set; }

        public double? CholesterolGrams { get; set; }

        public double? ChromiumGrams { get; set; }

        public double? CopperGrams { get; set; }

        public double? DietaryFiberGrams { get; set; }

        public double? FolateGrams { get; set; }

        public double? FolicAcidGrams { get; set; }

        public double? IodineGrams { get; set; }

        public double? IronGrams { get; set; }

        public double? MagnesiumGrams { get; set; }

        public double? ManganeseGrams { get; set; }

        public double? MolybdenumGrams { get; set; }

        public double? MonounsaturatedFatGrams { get; set; }

        public double? NiacinGrams { get; set; }

        public double? PantothenicAcidGrams { get; set; }

        public double? PhosphorusGrams { get; set; }

        public double? PolyunsaturatedFatGrams { get; set; }

        public double? PotassiumGrams { get; set; }

        public double? ProteinGrams { get; set; }

        public double? RiboflavinGrams { get; set; }

        public double? SaturatedFatGrams { get; set; }

        public double? SeleniumGrams { get; set; }

        public double? SodiumGrams { get; set; }

        public double? SugarGrams { get; set; }

        public double? ThiaminGrams { get; set; }

        public double? TotalCarbohydrateGrams { get; set; }

        public double? TotalFatGrams { get; set; }

        public double? TransFatGrams { get; set; }

        public double? UnsaturatedFatGrams { get; set; }

        public double? VitaminAGrams { get; set; }

        public double? VitaminB12Grams { get; set; }

        public double? VitaminB6Grams { get; set; }

        public double? VitaminCGrams { get; set; }

        public double? VitaminDGrams { get; set; }

        public double? VitaminEGrams { get; set; }

        public double? VitaminKGrams { get; set; }

        public double? ZincGrams { get; set; }
    }
}