using Common.Models;
using FluentValidation;

namespace Logic.Validators
{
    public class SleepSessionValidator : AbstractValidator<SleepSession>
    {
        public SleepSessionValidator(bool idIsRequired)
        {
            if (idIsRequired)
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }

            RuleFor(x => x.Source).NotEmpty();

            RuleFor(x => x.StartTimeUtc).LessThan(x => x.EndTimeUtc);

            RuleFor(x => x.DurationMinutes).GreaterThan(0);

            RuleFor(x => x.LastImportedAtUtc).NotEmpty();

            RuleFor(x => x.SleepStages).NotNull();

            RuleForEach(x => x.SleepStages).SetValidator(new SleepStageValidator(false));

            RuleFor(x => x.SleepStages)
                .Must((session, stages) => StagesAreWithinSession(session, stages))
                .WithMessage("One or more sleep stages are outside the session boundaries.");
        }

        private static bool StagesAreWithinSession(SleepSession session, ICollection<SleepStage>? stages)
        {
            if (stages == null || stages.Count == 0)
            {
                return true;
            }

            return stages.All(stage =>
                stage.StartTimeUtc >= session.StartTimeUtc &&
                stage.EndTimeUtc <= session.EndTimeUtc);
        }
    }
}