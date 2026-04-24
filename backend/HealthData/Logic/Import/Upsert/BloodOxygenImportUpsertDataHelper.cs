using Common.Import;
using Common.Models;

namespace Logic.Import.Upsert
{
    public static class BloodOxygenImportUpsertDataHelper
    {
        public static ImportUpsertData<BloodOxygenDay> BuildUpsertData(
            List<BloodOxygenDay> importedBloodOxygenDays,
            Dictionary<DateOnly, BloodOxygenDay> existingBloodOxygenDaysByDate)
        {
            var upsertData = new ImportUpsertData<BloodOxygenDay>();

            foreach (var importedBloodOxygenDay in importedBloodOxygenDays.OrderBy(x => x.Date))
            {
                if (existingBloodOxygenDaysByDate.TryGetValue(importedBloodOxygenDay.Date, out var existingBloodOxygenDay))
                {
                    if (HasChanges(existingBloodOxygenDay, importedBloodOxygenDay))
                    {
                        ApplyChanges(existingBloodOxygenDay, importedBloodOxygenDay);
                        upsertData.UpdatedItems.Add(existingBloodOxygenDay);
                    }
                    else
                    {
                        upsertData.UnchangedCount++;
                    }
                }
                else
                {
                    upsertData.InsertedItems.Add(importedBloodOxygenDay);
                }
            }

            return upsertData;
        }

        private static bool HasChanges(BloodOxygenDay existingBloodOxygenDay, BloodOxygenDay importedBloodOxygenDay)
        {
            return existingBloodOxygenDay.StartTimeUtc != importedBloodOxygenDay.StartTimeUtc
                   || existingBloodOxygenDay.EndTimeUtc != importedBloodOxygenDay.EndTimeUtc
                   || Math.Abs(existingBloodOxygenDay.AvgPercent - importedBloodOxygenDay.AvgPercent) > 0.0001d
                   || Math.Abs(existingBloodOxygenDay.MinPercent - importedBloodOxygenDay.MinPercent) > 0.0001d
                   || Math.Abs(existingBloodOxygenDay.MaxPercent - importedBloodOxygenDay.MaxPercent) > 0.0001d
                   || existingBloodOxygenDay.MeasurementCount != importedBloodOxygenDay.MeasurementCount;
        }

        private static void ApplyChanges(BloodOxygenDay existingBloodOxygenDay, BloodOxygenDay importedBloodOxygenDay)
        {
            existingBloodOxygenDay.StartTimeUtc = importedBloodOxygenDay.StartTimeUtc;
            existingBloodOxygenDay.EndTimeUtc = importedBloodOxygenDay.EndTimeUtc;
            existingBloodOxygenDay.AvgPercent = importedBloodOxygenDay.AvgPercent;
            existingBloodOxygenDay.MinPercent = importedBloodOxygenDay.MinPercent;
            existingBloodOxygenDay.MaxPercent = importedBloodOxygenDay.MaxPercent;
            existingBloodOxygenDay.MeasurementCount = importedBloodOxygenDay.MeasurementCount;
        }
    }
}