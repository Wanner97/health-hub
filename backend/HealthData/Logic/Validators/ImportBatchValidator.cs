using Common;
using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class ImportBatchValidator : AbstractValidator<ImportBatch>
    {
        public ImportBatchValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.ExportVersion).Equal(Const.LatestExportVersion);
            RuleFor(x => x.Source).NotEmpty();
            RuleFor(x => x.ExportedAtUtc).NotEmpty();
            RuleFor(x => x.ImportedAtUtc).NotEmpty();
            RuleFor(x => x.RangeStartUtc).LessThan(x => x.RangeEndUtc);
            RuleFor(x => x.ReceivedRecordCount).GreaterThan(0);
            RuleFor(x => x.InsertedRecordCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.UpdatedRecordCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.UnchangedRecordCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ActivityDayEntries).NotNull().Must(x => x.Count > 0).WithMessage("No activity records were found.");

            RuleFor(x => x.ReceivedRecordCount)
                .Must((batch, receivedRecordCount) =>
                    batch.ActivityDayEntries != null &&
                    receivedRecordCount == batch.ActivityDayEntries.Count)
                .WithMessage("ReceivedRecordCount does not match the number of activity records.");

            RuleForEach(x => x.ActivityDayEntries).SetValidator(new ActivityDayValidator(false));

            RuleFor(x => x.ActivityDayEntries).Must(HaveUniqueDates).WithMessage("The import contains duplicate dates.");
        }

        private static bool HaveUniqueDates(ICollection<ActivityDay>? activityDayEntries)
        {
            if (activityDayEntries == null)
            {
                return false;
            }

            return activityDayEntries
                .GroupBy(x => x.Date)
                .All(g => g.Count() == 1);
        }
    }
}