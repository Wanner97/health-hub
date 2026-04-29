using Common.Constants;
using Common.Models;

namespace Logic.Import.Helpers
{
    public static class ActivityDistanceCalculationHelper
    {
        private const double DefaultBodyHeightMeters = 1.8d;
        private const double StepLengthFactor = 0.415d;
        private const double DistanceToleranceMeters = 0.0001d;

        public static void FillMissingDistances(
            List<ActivityDay> activityDays,
            double? bodyHeightMeters)
        {
            var effectiveBodyHeightMeters = bodyHeightMeters.HasValue && bodyHeightMeters.Value > 0
                ? bodyHeightMeters.Value
                : DefaultBodyHeightMeters;

            var stepLengthMeters = CalculateStepLength(effectiveBodyHeightMeters);

            foreach (var activityDay in activityDays)
            {
                if (DistanceIsMissing(activityDay.DistanceMeters))
                {
                    activityDay.DistanceMeters = CalculateDistanceMeters(
                        activityDay.Steps,
                        stepLengthMeters);

                    activityDay.DistanceSource = ActivityDistanceSources.CalculatedFromSteps;
                    continue;
                }

                activityDay.DistanceSource = ActivityDistanceSources.HealthConnect;
            }
        }

        private static double CalculateStepLength(double bodyHeightMeters)
        {
            return bodyHeightMeters * StepLengthFactor;
        }

        private static double CalculateDistanceMeters(int steps, double stepLengthMeters)
        {
            return steps * stepLengthMeters;
        }

        private static bool DistanceIsMissing(double distanceMeters)
        {
            return Math.Abs(distanceMeters) < DistanceToleranceMeters;
        }
    }
}