using Common.Models;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Import
{
    public static class NutritionDayAggregationHelper
    {
        public static List<DateOnly> GetAffectedNutritionDates(
            List<NutritionRecord> insertedNutritionRecords,
            List<NutritionRecord> updatedNutritionRecords,
            List<DateOnly> originalUpdatedNutritionRecordDates)
        {
            return insertedNutritionRecords
                .Select(x => x.Date)
                .Concat(updatedNutritionRecords.Select(x => x.Date))
                .Concat(originalUpdatedNutritionRecordDates)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public static void RecalculateNutritionDays(
            AppDbContext context,
            ImportBatch importBatch,
            IReadOnlyCollection<DateOnly> affectedDates)
        {
            if (affectedDates.Count == 0)
            {
                return;
            }

            foreach (var affectedDate in affectedDates)
            {
                RecalculateNutritionDay(context, importBatch, affectedDate);
            }
        }

        private static void RecalculateNutritionDay(
            AppDbContext context,
            ImportBatch importBatch,
            DateOnly affectedDate)
        {
            var nutritionRecordsForDay = context.NutritionRecords
                .Include(x => x.Nutrients)
                .Where(x => x.Source == importBatch.Source && x.Date == affectedDate)
                .OrderBy(x => x.StartTimeUtc)
                .ToList();

            var existingNutritionDay = context.NutritionDays
                .Include(x => x.NutrientTotals)
                .Include(x => x.MealTypeSummaries)
                    .ThenInclude(x => x.NutrientTotals)
                .FirstOrDefault(x => x.Source == importBatch.Source && x.Date == affectedDate);

            if (existingNutritionDay != null)
            {
                RemoveExistingNutritionDayAggregates(context, existingNutritionDay);
                context.SaveChanges();
            }

            if (nutritionRecordsForDay.Count == 0)
            {
                if (existingNutritionDay != null)
                {
                    context.NutritionDays.Remove(existingNutritionDay);
                }

                return;
            }

            var nutritionDay = existingNutritionDay ?? new NutritionDay
            {
                Source = importBatch.Source,
                Date = affectedDate
            };

            nutritionDay.RecordCount = nutritionRecordsForDay.Count;
            nutritionDay.LastCalculatedAtUtc = importBatch.ImportedAtUtc;
            nutritionDay.LastImportBatchId = importBatch.Id;
            nutritionDay.LastImportBatch = importBatch;
            nutritionDay.NutrientTotals = BuildNutritionDayNutrientTotals(nutritionRecordsForDay);
            nutritionDay.MealTypeSummaries = BuildNutritionMealTypeSummaries(nutritionRecordsForDay);

            foreach (var nutritionRecord in nutritionRecordsForDay)
            {
                nutritionRecord.NutritionDay = nutritionDay;
            }

            if (existingNutritionDay == null)
            {
                context.NutritionDays.Add(nutritionDay);
            }
            else
            {
                context.NutritionDays.Update(nutritionDay);
            }
        }

        private static List<NutritionDayNutrientTotal> BuildNutritionDayNutrientTotals(
            List<NutritionRecord> nutritionRecords)
        {
            return nutritionRecords
                .SelectMany(x => x.Nutrients)
                .GroupBy(x => x.NutrientKey)
                .Select(group => new NutritionDayNutrientTotal
                {
                    NutrientKey = group.Key,
                    TotalAmount = group.Sum(x => x.Amount),
                    Unit = group.First().Unit
                })
                .OrderBy(x => x.NutrientKey)
                .ToList();
        }

        private static List<NutritionMealTypeSummary> BuildNutritionMealTypeSummaries(
            List<NutritionRecord> nutritionRecords)
        {
            return nutritionRecords
                .GroupBy(x => x.MealType)
                .Select(mealTypeGroup => new NutritionMealTypeSummary
                {
                    MealType = mealTypeGroup.Key,
                    RecordCount = mealTypeGroup.Count(),
                    NutrientTotals = mealTypeGroup
                        .SelectMany(x => x.Nutrients)
                        .GroupBy(x => x.NutrientKey)
                        .Select(nutrientGroup => new NutritionMealTypeNutrientTotal
                        {
                            NutrientKey = nutrientGroup.Key,
                            TotalAmount = nutrientGroup.Sum(x => x.Amount),
                            Unit = nutrientGroup.First().Unit
                        })
                        .OrderBy(x => x.NutrientKey)
                        .ToList()
                })
                .OrderBy(x => x.MealType)
                .ToList();
        }

        private static void RemoveExistingNutritionDayAggregates(
            AppDbContext context,
            NutritionDay existingNutritionDay)
        {
            var existingMealTypeNutrientTotals = existingNutritionDay.MealTypeSummaries
                .SelectMany(x => x.NutrientTotals)
                .ToList();

            if (existingMealTypeNutrientTotals.Count > 0)
            {
                context.NutritionMealTypeNutrientTotals.RemoveRange(existingMealTypeNutrientTotals);
            }

            if (existingNutritionDay.MealTypeSummaries.Count > 0)
            {
                context.NutritionMealTypeSummaries.RemoveRange(existingNutritionDay.MealTypeSummaries);
            }

            if (existingNutritionDay.NutrientTotals.Count > 0)
            {
                context.NutritionDayNutrientTotals.RemoveRange(existingNutritionDay.NutrientTotals);
            }

            existingNutritionDay.MealTypeSummaries = new List<NutritionMealTypeSummary>();
            existingNutritionDay.NutrientTotals = new List<NutritionDayNutrientTotal>();
        }
    }
}