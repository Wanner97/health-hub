using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class SleepStageValidator : AbstractValidator<SleepStage>
    {
        public SleepStageValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Stage).NotEmpty();

            RuleFor(x => x.StartTimeUtc).LessThan(x => x.EndTimeUtc);
        }
    }
}