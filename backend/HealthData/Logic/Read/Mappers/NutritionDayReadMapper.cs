using Common.Constants;
using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Read.Mappers
{
    public static class NutritionDayReadMapper
    {
        private const double CarbohydrateCaloriesPerGram = 4d;
        private const double ProteinCaloriesPerGram = 4d;
        private const double FatCaloriesPerGram = 9d;

        public static List<NutritionDayReadDto> MapToReadDtos(List<NutritionDay> nutritionDays)
        {
            return nutritionDays
                .Select(MapToReadDto)
                .ToList();
        }

        private static NutritionDayReadDto MapToReadDto(NutritionDay nutritionDay)
        {
            var totalEnergyKcal = GetTotalAmount(nutritionDay.NutrientTotals, NutritionNutrientKeys.EnergyKcal);

            var totalCarbohydrateGrams = GetTotalAmount(nutritionDay.NutrientTotals, NutritionNutrientKeys.TotalCarbohydrateGrams);

            var totalFatGrams = GetTotalAmount(nutritionDay.NutrientTotals, NutritionNutrientKeys.TotalFatGrams);

            var totalProteinGrams = GetTotalAmount(nutritionDay.NutrientTotals, NutritionNutrientKeys.ProteinGrams);

            var carbohydrateCalories = totalCarbohydrateGrams * CarbohydrateCaloriesPerGram;
            var fatCalories = totalFatGrams * FatCaloriesPerGram;
            var proteinCalories = totalProteinGrams * ProteinCaloriesPerGram;

            var macroCalories = carbohydrateCalories + fatCalories + proteinCalories;

            return new NutritionDayReadDto
            {
                Date = nutritionDay.Date,
                RecordCount = nutritionDay.RecordCount,

                TotalEnergyKcal = totalEnergyKcal,
                TotalCarbohydrateGrams = totalCarbohydrateGrams,
                TotalFatGrams = totalFatGrams,
                TotalProteinGrams = totalProteinGrams,

                CarbohydratePercent = CalculatePercent(carbohydrateCalories, macroCalories),
                FatPercent = CalculatePercent(fatCalories, macroCalories),
                ProteinPercent = CalculatePercent(proteinCalories, macroCalories),

                Records = nutritionDay.Records
                    .OrderBy(x => x.StartTimeUtc)
                    .Select(MapToRecordReadDto)
                    .ToList()
            };
        }

        private static NutritionRecordReadDto MapToRecordReadDto(NutritionRecord nutritionRecord)
        {
            return new NutritionRecordReadDto
            {
                HealthConnectRecordId = nutritionRecord.HealthConnectRecordId,
                Date = nutritionRecord.Date,
                StartTimeUtc = nutritionRecord.StartTimeUtc,
                EndTimeUtc = nutritionRecord.EndTimeUtc,
                MealType = nutritionRecord.MealType,
                Name = nutritionRecord.Name,

                TotalEnergyKcal = GetAmount(nutritionRecord.Nutrients, NutritionNutrientKeys.EnergyKcal),

                TotalCarbohydrateGrams = GetAmount(nutritionRecord.Nutrients, NutritionNutrientKeys.TotalCarbohydrateGrams),

                TotalFatGrams = GetAmount(nutritionRecord.Nutrients, NutritionNutrientKeys.TotalFatGrams),

                TotalProteinGrams = GetAmount(nutritionRecord.Nutrients, NutritionNutrientKeys.ProteinGrams),

                Nutrients = nutritionRecord.Nutrients
                    .OrderBy(x => x.NutrientKey)
                    .Select(MapToNutrientReadDto)
                    .ToList()
            };
        }

        private static NutritionNutrientReadDto MapToNutrientReadDto(NutritionRecordNutrient nutrient)
        {
            return new NutritionNutrientReadDto
            {
                NutrientKey = nutrient.NutrientKey,
                Amount = nutrient.Amount,
                Unit = nutrient.Unit
            };
        }

        private static double GetTotalAmount(IEnumerable<NutritionDayNutrientTotal> nutrientTotals, string nutrientKey)
        {
            return nutrientTotals.FirstOrDefault(x => x.NutrientKey == nutrientKey)?.TotalAmount ?? 0d;
        }

        private static double GetAmount(IEnumerable<NutritionRecordNutrient> nutrients, string nutrientKey)
        {
            return nutrients.FirstOrDefault(x => x.NutrientKey == nutrientKey)?.Amount ?? 0d;
        }

        private static double CalculatePercent(double value, double total)
        {
            if (total <= 0)
            {
                return 0d;
            }

            return value / total * 100d;
        }
    }
}