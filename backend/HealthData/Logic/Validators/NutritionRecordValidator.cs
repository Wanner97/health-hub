using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class NutritionRecordValidator : AbstractValidator<NutritionRecord>
    {
        public NutritionRecordValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty().MaximumLength(100);
            RuleFor(x => x.HealthConnectRecordId).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Date).Must(x => x != default).WithMessage("Date is required.");
            RuleFor(x => x.StartTimeUtc).LessThanOrEqualTo(x => x.EndTimeUtc);
            RuleFor(x => x.MealType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).MaximumLength(500);
            RuleFor(x => x.LastImportedAtUtc).NotEmpty();
            RuleFor(x => x.Nutrients).NotNull();
            RuleFor(x => x.Nutrients).Must(HaveUniqueNutrientKeys).WithMessage("Nutrition record contains duplicate nutrient keys.");

            RuleForEach(x => x.Nutrients).SetValidator(new NutritionRecordNutrientValidator(false));
        }

        private static bool HaveUniqueNutrientKeys(ICollection<NutritionRecordNutrient>? nutrients)
        {
            if (nutrients == null)
            {
                return true;
            }

            return nutrients
                .GroupBy(x => x.NutrientKey)
                .All(g => g.Count() == 1);
        }
    }
}