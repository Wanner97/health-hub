using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class BloodOxygenDayValidator : AbstractValidator<BloodOxygenDay>
    {
        public BloodOxygenDayValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty();

            RuleFor(x => x.Date).Must(x => x != default).WithMessage("Date is required.");

            RuleFor(x => x.StartTimeUtc).LessThan(x => x.EndTimeUtc);

            RuleFor(x => x.AvgPercent).InclusiveBetween(0d, 100d);

            RuleFor(x => x.MinPercent).InclusiveBetween(0d, 100d);

            RuleFor(x => x.MaxPercent).InclusiveBetween(0d, 100d);

            RuleFor(x => x.MeasurementCount).GreaterThan(0);

            RuleFor(x => x.MinPercent).LessThanOrEqualTo(x => x.AvgPercent);

            RuleFor(x => x.AvgPercent).LessThanOrEqualTo(x => x.MaxPercent);
        }
    }
}