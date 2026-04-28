using Common.Constants;
using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class ActivityDayValidator : AbstractValidator<ActivityDay>
    {
        public ActivityDayValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty();

            RuleFor(x => x.Date).Must(x => x != default).WithMessage("Date is required.");

            RuleFor(x => x.StartTimeUtc).LessThan(x => x.EndTimeUtc);

            RuleFor(x => x.Steps).GreaterThanOrEqualTo(0);

            RuleFor(x => x.DistanceMeters).GreaterThanOrEqualTo(0);

            RuleFor(x => x.TotalCaloriesBurnedKcal).GreaterThanOrEqualTo(0);

            RuleFor(x => x.DistanceSource).NotEmpty();

            RuleFor(x => x.DistanceSource)
                .Must(ActivityDistanceSources.SupportedValues.Contains)
                .WithMessage("DistanceSource contains an unsupported value.");
        }
    }
}