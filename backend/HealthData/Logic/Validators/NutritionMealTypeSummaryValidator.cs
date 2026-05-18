using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class NutritionMealTypeSummaryValidator : AbstractValidator<NutritionMealTypeSummary>
    {
        public NutritionMealTypeSummaryValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.MealType).NotEmpty().MaximumLength(50);

            RuleFor(x => x.RecordCount).GreaterThan(0);

            RuleFor(x => x.NutrientTotals).NotNull();

            RuleFor(x => x.NutrientTotals).Must(HaveUniqueNutrientKeys).WithMessage("Nutrition meal type summary contains duplicate nutrient keys.");

            RuleForEach(x => x.NutrientTotals).SetValidator(new NutritionMealTypeNutrientTotalValidator(false));
        }

        private static bool HaveUniqueNutrientKeys(ICollection<NutritionMealTypeNutrientTotal>? nutrientTotals)
        {
            if (nutrientTotals == null)
            {
                return true;
            }

            return nutrientTotals
                .GroupBy(x => x.NutrientKey)
                .All(g => g.Count() == 1);
        }
    }
}