using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class HeartRateDayValidator : AbstractValidator<HeartRateDay>
    {
        public HeartRateDayValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty();

            RuleFor(x => x.Date).Must(x => x != default).WithMessage("Date is required.");

            RuleFor(x => x.StartTimeUtc).LessThan(x => x.EndTimeUtc);

            RuleFor(x => x.AvgBpm).GreaterThan(0);

            RuleFor(x => x.MinBpm).GreaterThan(0);

            RuleFor(x => x.MaxBpm).GreaterThan(0);

            RuleFor(x => x.MeasurementCount).GreaterThan(0);

            RuleFor(x => x.MinBpm).LessThanOrEqualTo(x => x.AvgBpm);

            RuleFor(x => x.AvgBpm).LessThanOrEqualTo(x => x.MaxBpm);

            RuleFor(x => x.HourlyRecords).NotNull();

            RuleForEach(x => x.HourlyRecords).SetValidator(new HeartRateHourlyRecordValidator(false));

            RuleFor(x => x.HourlyRecords).Must(HaveUniqueHours).WithMessage("The heart rate day contains duplicate hours.");
        }

        private static bool HaveUniqueHours(ICollection<HeartRateHourlyRecord>? hourlyRecords)
        {
            if (hourlyRecords == null)
            {
                return true;
            }

            return hourlyRecords
                .GroupBy(x => x.Hour)
                .All(g => g.Count() == 1);
        }
    }
}