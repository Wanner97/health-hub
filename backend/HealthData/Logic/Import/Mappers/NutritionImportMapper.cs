using Common.Constants;
using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Import.Mappers
{
    public static class NutritionImportMapper
    {
        public static List<NutritionRecord> MapToNutritionRecords(string source, DateTime importedAtUtc, NutritionClusterDto? nutritionCluster)
        {
            if (nutritionCluster?.Records == null || nutritionCluster.Records.Count == 0)
            {
                return new List<NutritionRecord>();
            }

            return nutritionCluster.Records
                .Select(x => MapToNutritionRecord(source, importedAtUtc, x))
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTimeUtc)
                .ToList();
        }

        private static NutritionRecord MapToNutritionRecord(string source, DateTime importedAtUtc, NutritionRecordDto nutritionRecordDto)
        {
            var nutritionRecord = new NutritionRecord
            {
                Source = source,
                HealthConnectRecordId = nutritionRecordDto.HealthConnectRecordId?.Trim() ?? string.Empty,
                Date = nutritionRecordDto.Date,
                StartTimeUtc = nutritionRecordDto.StartTime.UtcDateTime,
                EndTimeUtc = nutritionRecordDto.EndTime.UtcDateTime,
                MealType = NormalizeMealType(nutritionRecordDto.MealType),
                Name = NormalizeName(nutritionRecordDto.Name),
                LastImportedAtUtc = importedAtUtc
            };

            AddNutrientsFromDto(nutritionRecord.Nutrients, nutritionRecordDto);

            return nutritionRecord;
        }

        private static string NormalizeMealType(string? mealType)
        {
            if (string.IsNullOrWhiteSpace(mealType))
            {
                return "unknown";
            }

            return mealType.Trim();
        }

        private static string? NormalizeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return name.Trim();
        }

        private static void AddNutrientsFromDto(List<NutritionRecordNutrient> nutrients, NutritionRecordDto nutritionRecordDto)
        {
            AddNutrient(nutrients, NutritionNutrientKeys.EnergyKcal, nutritionRecordDto.EnergyKcal, NutritionUnits.Kcal);
            AddNutrient(nutrients, NutritionNutrientKeys.EnergyFromFatKcal, nutritionRecordDto.EnergyFromFatKcal, NutritionUnits.Kcal);

            AddNutrient(nutrients, NutritionNutrientKeys.BiotinGrams, nutritionRecordDto.BiotinGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.CaffeineGrams, nutritionRecordDto.CaffeineGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.CalciumGrams, nutritionRecordDto.CalciumGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.ChlorideGrams, nutritionRecordDto.ChlorideGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.CholesterolGrams, nutritionRecordDto.CholesterolGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.ChromiumGrams, nutritionRecordDto.ChromiumGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.CopperGrams, nutritionRecordDto.CopperGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.DietaryFiberGrams, nutritionRecordDto.DietaryFiberGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.FolateGrams, nutritionRecordDto.FolateGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.FolicAcidGrams, nutritionRecordDto.FolicAcidGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.IodineGrams, nutritionRecordDto.IodineGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.IronGrams, nutritionRecordDto.IronGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.MagnesiumGrams, nutritionRecordDto.MagnesiumGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.ManganeseGrams, nutritionRecordDto.ManganeseGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.MolybdenumGrams, nutritionRecordDto.MolybdenumGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.MonounsaturatedFatGrams, nutritionRecordDto.MonounsaturatedFatGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.NiacinGrams, nutritionRecordDto.NiacinGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.PantothenicAcidGrams, nutritionRecordDto.PantothenicAcidGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.PhosphorusGrams, nutritionRecordDto.PhosphorusGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.PolyunsaturatedFatGrams, nutritionRecordDto.PolyunsaturatedFatGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.PotassiumGrams, nutritionRecordDto.PotassiumGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.ProteinGrams, nutritionRecordDto.ProteinGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.RiboflavinGrams, nutritionRecordDto.RiboflavinGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.SaturatedFatGrams, nutritionRecordDto.SaturatedFatGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.SeleniumGrams, nutritionRecordDto.SeleniumGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.SodiumGrams, nutritionRecordDto.SodiumGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.SugarGrams, nutritionRecordDto.SugarGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.ThiaminGrams, nutritionRecordDto.ThiaminGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.TotalCarbohydrateGrams, nutritionRecordDto.TotalCarbohydrateGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.TotalFatGrams, nutritionRecordDto.TotalFatGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.TransFatGrams, nutritionRecordDto.TransFatGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.UnsaturatedFatGrams, nutritionRecordDto.UnsaturatedFatGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.VitaminAGrams, nutritionRecordDto.VitaminAGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.VitaminB12Grams, nutritionRecordDto.VitaminB12Grams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.VitaminB6Grams, nutritionRecordDto.VitaminB6Grams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.VitaminCGrams, nutritionRecordDto.VitaminCGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.VitaminDGrams, nutritionRecordDto.VitaminDGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.VitaminEGrams, nutritionRecordDto.VitaminEGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.VitaminKGrams, nutritionRecordDto.VitaminKGrams, NutritionUnits.Grams);
            AddNutrient(nutrients, NutritionNutrientKeys.ZincGrams, nutritionRecordDto.ZincGrams, NutritionUnits.Grams);
        }

        private static void AddNutrient(List<NutritionRecordNutrient> nutrients, string nutrientKey, double? amount, string unit)
        {
            if (!amount.HasValue)
            {
                return;
            }

            nutrients.Add(new NutritionRecordNutrient
            {
                NutrientKey = nutrientKey,
                Amount = amount.Value,
                Unit = unit
            });
        }
    }
}