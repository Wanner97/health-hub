using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class NutritionRecordNutrientValidator : AbstractValidator<NutritionRecordNutrient>
    {
        public NutritionRecordNutrientValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.NutrientKey).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Unit).NotEmpty().MaximumLength(20);
        }
    }
}