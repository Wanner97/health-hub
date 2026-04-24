using Common.Import;
using Common.Models;

namespace Logic.Import.Upsert
{
    public static class ActivityImportUpsertDataHelper
    {
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
                   || Math.Abs(existingActivityDay.DistanceMeters - importedActivityDay.DistanceMeters) > 0.0001d;
        }

        private static void ApplyChanges(ActivityDay existingActivityDay, ActivityDay importedActivityDay)
        {
            existingActivityDay.StartTimeUtc = importedActivityDay.StartTimeUtc;
            existingActivityDay.EndTimeUtc = importedActivityDay.EndTimeUtc;
            existingActivityDay.Steps = importedActivityDay.Steps;
            existingActivityDay.DistanceMeters = importedActivityDay.DistanceMeters;
        }
    }
}