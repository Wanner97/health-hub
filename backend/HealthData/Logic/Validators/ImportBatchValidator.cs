using Common;
using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class ImportBatchValidator : AbstractValidator<ImportBatch>
    {
        public ImportBatchValidator(bool idIsRequired, string expectedExportVersion)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.ExportVersion).Equal(expectedExportVersion);
            RuleFor(x => x.Source).NotEmpty();
            RuleFor(x => x.ExportType).NotEmpty();

            RuleFor(x => x.ExportedAtUtc).NotEmpty();
            RuleFor(x => x.ImportedAtUtc).NotEmpty();
            RuleFor(x => x.RangeStartUtc).LessThan(x => x.RangeEndUtc);

            RuleFor(x => x.ReceivedRecordCount).GreaterThan(0);
            RuleFor(x => x.InsertedRecordCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.UpdatedRecordCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.UnchangedRecordCount).GreaterThanOrEqualTo(0);

            RuleFor(x => x).Must(HaveAtLeastOneImportEntry).WithMessage("No import records were found.");

            RuleFor(x => x.ReceivedRecordCount)
                .Must((batch, receivedRecordCount) =>
                    receivedRecordCount ==
                    (batch.ActivityDayEntries?.Count ?? 0) +
                    (batch.SleepSessionEntries?.Count ?? 0) +
                    (batch.HeartRateDayEntries?.Count ?? 0) +
                    (batch.HeartRateDayEntries?.Sum(x => x.HourlyRecords.Count) ?? 0) +
                    (batch.BloodOxygenDayEntries?.Count ?? 0))
                .WithMessage("ReceivedRecordCount does not match the number of imported records.");

            RuleForEach(x => x.ActivityDayEntries).SetValidator(new ActivityDayValidator(false));

            RuleForEach(x => x.SleepSessionEntries).SetValidator(new SleepSessionValidator(false));

            RuleForEach(x => x.HeartRateDayEntries).SetValidator(new HeartRateDayValidator(false));

            RuleForEach(x => x.BloodOxygenDayEntries).SetValidator(new BloodOxygenDayValidator(false));

            RuleFor(x => x.ActivityDayEntries)
                .Must(HaveUniqueActivityDates)
                .When(x => x.ActivityDayEntries != null && x.ActivityDayEntries.Count > 0)
                .WithMessage("The import contains duplicate activity dates.");

            RuleFor(x => x.SleepSessionEntries)
                .Must(HaveUniqueStartTimes)
                .When(x => x.SleepSessionEntries != null && x.SleepSessionEntries.Count > 0)
                .WithMessage("The import contains duplicate sleep session start times after consolidation.");

            RuleFor(x => x.HeartRateDayEntries)
                .Must(HaveUniqueHeartRateDates)
                .When(x => x.HeartRateDayEntries != null && x.HeartRateDayEntries.Count > 0)
                .WithMessage("The import contains duplicate heart rate dates.");

            RuleFor(x => x.BloodOxygenDayEntries)
                .Must(HaveUniqueBloodOxygenDates)
                .When(x => x.BloodOxygenDayEntries != null && x.BloodOxygenDayEntries.Count > 0)
                .WithMessage("The import contains duplicate blood oxygen dates.");
        }

        private static bool HaveAtLeastOneImportEntry(ImportBatch importBatch)
        {
            return (importBatch.ActivityDayEntries?.Count ?? 0) > 0
                   || (importBatch.SleepSessionEntries?.Count ?? 0) > 0
                   || (importBatch.HeartRateDayEntries?.Count ?? 0) > 0;
        }

        private static bool HaveUniqueActivityDates(ICollection<ActivityDay>? activityDayEntries)
        {
            if (activityDayEntries == null)
            {
                return true;
            }

            return activityDayEntries
                .GroupBy(x => x.Date)
                .All(g => g.Count() == 1);
        }

        private static bool HaveUniqueStartTimes(ICollection<SleepSession>? sleepSessionEntries)
        {
            if (sleepSessionEntries == null)
            {
                return true;
            }

            return sleepSessionEntries
                .GroupBy(x => x.StartTimeUtc)
                .All(g => g.Count() == 1);
        }

        private static bool HaveUniqueHeartRateDates(ICollection<HeartRateDay>? heartRateDayEntries)
        {
            if (heartRateDayEntries == null)
            {
                return true;
            }

            return heartRateDayEntries
                .GroupBy(x => x.Date)
                .All(g => g.Count() == 1);
        }

        private static bool HaveUniqueBloodOxygenDates(ICollection<BloodOxygenDay>? bloodOxygenDayEntries)
        {
            if (bloodOxygenDayEntries == null)
            {
                return true;
            }

            return bloodOxygenDayEntries
                .GroupBy(x => x.Date)
                .All(g => g.Count() == 1);
        }
    }
}