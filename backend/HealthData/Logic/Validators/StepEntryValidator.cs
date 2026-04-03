using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class StepEntryValidator : AbstractValidator<StepEntry>
    {
        public StepEntryValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }
            
            RuleFor(x => x.Count).GreaterThanOrEqualTo(0);
            RuleFor(x => x.StartTime).LessThan(x => x.EndTime);
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        }
    }   
}