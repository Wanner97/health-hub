using Common.Constants;
using Common.Import;
using Common.Models;

namespace Logic.Import.Upsert
{
    public static class ActivityImportUpsertDataHelper
    {
        private const double DistanceToleranceMeters = 0.0001d;

        public static ImportUpsertData<ActivityDay> BuildUpsertData(
            List<ActivityDay> importedActivityDays,
            Dictionary<DateOnly, ActivityDay> existingActivityDaysByDate)
        {
            var upsertData = new ImportUpsertData<ActivityDay>();

            foreach (var importedActivityDay in importedActivityDays)
            {
                if (existingActivityDaysByDate.TryGetValue(importedActivityDay.Date, out var existingActivityDay))
                {
                    if (HasChanges(existingActivityDay, importedActivityDay))
                    {
                        ApplyChanges(existingActivityDay, importedActivityDay);
                        upsertData.UpdatedItems.Add(existingActivityDay);
                    }
                    else
                    {
                        upsertData.UnchangedCount++;
                    }
                }
                else
                {
                    upsertData.InsertedItems.Add(importedActivityDay);
                }
            }

            return upsertData;
        }

        private static bool HasChanges(ActivityDay existingActivityDay, ActivityDay importedActivityDay)
        {
            return existingActivityDay.StartTimeUtc != importedActivityDay.StartTimeUtc
                   || existingActivityDay.EndTimeUtc != importedActivityDay.EndTimeUtc
                   || existingActivityDay.Steps != importedActivityDay.Steps
                   || HasRelevantDistanceChanges(existingActivityDay, importedActivityDay);
        }

        private static bool HasRelevantDistanceChanges(ActivityDay existingActivityDay, ActivityDay importedActivityDay)
        {
            if (!CanReplaceDistance(existingActivityDay, importedActivityDay))
            {
                return false;
            }

            return Math.Abs(existingActivityDay.DistanceMeters - importedActivityDay.DistanceMeters) > DistanceToleranceMeters
                   || existingActivityDay.DistanceSource != importedActivityDay.DistanceSource;
        }

        private static void ApplyChanges(ActivityDay existingActivityDay, ActivityDay importedActivityDay)
        {
            existingActivityDay.StartTimeUtc = importedActivityDay.StartTimeUtc;
            existingActivityDay.EndTimeUtc = importedActivityDay.EndTimeUtc;
            existingActivityDay.Steps = importedActivityDay.Steps;

            if (CanReplaceDistance(existingActivityDay, importedActivityDay))
            {
                existingActivityDay.DistanceMeters = importedActivityDay.DistanceMeters;
                existingActivityDay.DistanceSource = importedActivityDay.DistanceSource;
            }
        }

        private static bool CanReplaceDistance(ActivityDay existingActivityDay, ActivityDay importedActivityDay)
        {
            return !IsHealthConnectDistance(existingActivityDay.DistanceSource)
                   || !IsCalculatedDistance(importedActivityDay.DistanceSource);
        }

        private static bool IsHealthConnectDistance(string? distanceSource)
        {
            return string.Equals(
                distanceSource,
                ActivityDistanceSources.HealthConnect,
                StringComparison.Ordinal);
        }

        private static bool IsCalculatedDistance(string? distanceSource)
        {
            return string.Equals(
                distanceSource,
                ActivityDistanceSources.CalculatedFromSteps,
                StringComparison.Ordinal);
        }
    }
}