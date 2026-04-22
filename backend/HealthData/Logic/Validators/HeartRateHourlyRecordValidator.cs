using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class HeartRateHourlyRecordValidator : AbstractValidator<HeartRateHourlyRecord>
    {
        public HeartRateHourlyRecordValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Hour).InclusiveBetween(0, 23);

            RuleFor(x => x.StartTimeUtc).LessThan(x => x.EndTimeUtc);

            RuleFor(x => x.AvgBpm).GreaterThan(0);

            RuleFor(x => x.MinBpm).GreaterThan(0);

            RuleFor(x => x.MaxBpm).GreaterThan(0);

            RuleFor(x => x.MeasurementCount).GreaterThan(0);

            RuleFor(x => x.MinBpm).LessThanOrEqualTo(x => x.AvgBpm);

            RuleFor(x => x.AvgBpm).LessThanOrEqualTo(x => x.MaxBpm);
        }
    }
}