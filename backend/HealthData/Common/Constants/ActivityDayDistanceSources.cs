namespace Common.Constants
{
    public static class ActivityDistanceSources
    {
        public const string HealthConnect = "health_connect";

        public const string CalculatedFromSteps = "calculated_from_steps";

        public static readonly IReadOnlyCollection<string> SupportedValues = new[]
        {
            HealthConnect,
            CalculatedFromSteps
        };
    }
}