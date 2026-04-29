using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class HeightMeasurementValidator : AbstractValidator<HeightMeasurement>
    {
        public HeightMeasurementValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty();

            RuleFor(x => x.HeightCm).GreaterThan(0);

            RuleFor(x => x.MeasuredAtUtc).NotEmpty();

            RuleFor(x => x.LastImportedAtUtc).NotEmpty();
        }
    }
}