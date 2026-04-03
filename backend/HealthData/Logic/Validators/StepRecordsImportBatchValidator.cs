using Common;
using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class StepRecordsImportBatchValidator : AbstractValidator<StepRecordsImportBatch>
    {
        public StepRecordsImportBatchValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }
            
            RuleFor(x => x.ExportVersion).GreaterThanOrEqualTo(Const.LatestExportVersion);
            RuleFor(x => x.Source).NotEmpty();
            RuleFor(x => x.RecordCount).GreaterThan(0);
            RuleFor(x => x.StepEntries).Must(HaveUniqueDates);
        }
        
        private static bool HaveUniqueDates(ICollection<StepEntry>? stepEntries)
        {
            if (stepEntries == null)
            {
                return false;
            }

            return stepEntries
                .GroupBy(x => x.Date)
                .All(g => g.Count() == 1);
        }
    }
}