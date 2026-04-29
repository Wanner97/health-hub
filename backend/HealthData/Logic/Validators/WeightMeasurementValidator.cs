using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class WeightMeasurementValidator : AbstractValidator<WeightMeasurement>
    {
        public WeightMeasurementValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty();

            RuleFor(x => x.Date)
                .Must(x => x != default)
                .WithMessage("Date is required.");

            RuleFor(x => x.WeightKg).GreaterThan(0);

            RuleFor(x => x.MeasuredAtUtc).NotEmpty();

            RuleFor(x => x.LastImportedAtUtc).NotEmpty();
        }
    }
}