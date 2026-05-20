using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IHealthImportExistingDataLoader
    {
        Dictionary<DateOnly, ActivityDay> GetExistingActivityDays(string source, IEnumerable<DateOnly> dates);

        Dictionary<DateTime, SleepSession> GetExistingSleepSessions(string source, IEnumerable<DateTime> startTimes);

        Dictionary<DateOnly, HeartRateDay> GetExistingHeartRateDays(string source, IEnumerable<DateOnly> dates);

        Dictionary<DateOnly, BloodOxygenDay> GetExistingBloodOxygenDays(string source, IEnumerable<DateOnly> dates);

        Dictionary<DateTime, HeightMeasurement> GetExistingHeightMeasurements(string source, IEnumerable<DateTime> measuredAtUtcValues);

        Dictionary<DateTime, WeightMeasurement> GetExistingWeightMeasurements(string source, IEnumerable<DateTime> measuredAtUtcValues);

        Dictionary<string, NutritionRecord> GetExistingNutritionRecords(string source, IEnumerable<string> healthConnectRecordIds);
    }
}