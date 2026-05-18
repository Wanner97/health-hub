using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class NutritionDayValidator : AbstractValidator<NutritionDay>
    {
        public NutritionDayValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Date).Must(x => x != default).WithMessage("Date is required.");

            RuleFor(x => x.RecordCount).GreaterThan(0);

            RuleFor(x => x.LastCalculatedAtUtc).NotEmpty();

            RuleFor(x => x.NutrientTotals).NotNull();

            RuleFor(x => x.NutrientTotals).Must(HaveUniqueNutrientKeys).WithMessage("Nutrition day contains duplicate nutrient keys.");

            RuleForEach(x => x.NutrientTotals).SetValidator(new NutritionDayNutrientTotalValidator(false));

            RuleFor(x => x.MealTypeSummaries).NotNull();

            RuleFor(x => x.MealTypeSummaries).Must(HaveUniqueMealTypes).WithMessage("Nutrition day contains duplicate meal types.");

            RuleForEach(x => x.MealTypeSummaries).SetValidator(new NutritionMealTypeSummaryValidator(false));
        }

        private static bool HaveUniqueNutrientKeys(ICollection<NutritionDayNutrientTotal>? nutrientTotals)
        {
            if (nutrientTotals == null)
            {
                return true;
            }

            return nutrientTotals
                .GroupBy(x => x.NutrientKey)
                .All(g => g.Count() == 1);
        }

        private static bool HaveUniqueMealTypes(ICollection<NutritionMealTypeSummary>? mealTypeSummaries)
        {
            if (mealTypeSummaries == null)
            {
                return true;
            }

            return mealTypeSummaries
                .GroupBy(x => x.MealType)
                .All(g => g.Count() == 1);
        }
    }
}