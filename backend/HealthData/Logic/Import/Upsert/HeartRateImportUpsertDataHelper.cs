using Common.Import;
using Common.Models;

namespace Logic.Import.Upsert
{
    public static class HeartRateImportUpsertDataHelper
    {
        public static ImportUpsertData<HeartRateDay> BuildUpsertData(
            List<HeartRateDay> importedHeartRateDays,
            Dictionary<DateOnly, HeartRateDay> existingHeartRateDaysByDate)
        {
            var upsertData = new ImportUpsertData<HeartRateDay>();

            foreach (var importedHeartRateDay in importedHeartRateDays.OrderBy(x => x.Date))
            {
                if (existingHeartRateDaysByDate.TryGetValue(importedHeartRateDay.Date, out var existingHeartRateDay))
                {
                    if (HasChanges(existingHeartRateDay, importedHeartRateDay))
                    {
                        ApplyChanges(existingHeartRateDay, importedHeartRateDay);
                        upsertData.UpdatedItems.Add(existingHeartRateDay);
                    }
                    else
                    {
                        upsertData.UnchangedCount++;
                    }
                }
                else
                {
                    upsertData.InsertedItems.Add(importedHeartRateDay);
                }
            }

            return upsertData;
        }

        private static bool HasChanges(HeartRateDay existingHeartRateDay, HeartRateDay importedHeartRateDay)
        {
            return existingHeartRateDay.StartTimeUtc != importedHeartRateDay.StartTimeUtc
                   || existingHeartRateDay.EndTimeUtc != importedHeartRateDay.EndTimeUtc
                   || existingHeartRateDay.AvgBpm != importedHeartRateDay.AvgBpm
                   || existingHeartRateDay.MinBpm != importedHeartRateDay.MinBpm
                   || existingHeartRateDay.MaxBpm != importedHeartRateDay.MaxBpm
                   || existingHeartRateDay.MeasurementCount != importedHeartRateDay.MeasurementCount
                   || !HaveSameHourlyRecords(existingHeartRateDay.HourlyRecords, importedHeartRateDay.HourlyRecords);
        }

        private static bool HaveSameHourlyRecords(
            ICollection<HeartRateHourlyRecord> existingHourlyRecords,
            ICollection<HeartRateHourlyRecord> importedHourlyRecords)
        {
            var normalizedExistingHourlyRecords = existingHourlyRecords
                .OrderBy(x => x.Hour)
                .ToList();

            var normalizedImportedHourlyRecords = importedHourlyRecords
                .OrderBy(x => x.Hour)
                .ToList();

            if (normalizedExistingHourlyRecords.Count != normalizedImportedHourlyRecords.Count)
            {
                return false;
            }

            for (var i = 0; i < normalizedExistingHourlyRecords.Count; i++)
            {
                var existingHourlyRecord = normalizedExistingHourlyRecords[i];
                var importedHourlyRecord = normalizedImportedHourlyRecords[i];

                if (existingHourlyRecord.Hour != importedHourlyRecord.Hour
                    || existingHourlyRecord.StartTimeUtc != importedHourlyRecord.StartTimeUtc
                    || existingHourlyRecord.EndTimeUtc != importedHourlyRecord.EndTimeUtc
                    || existingHourlyRecord.AvgBpm != importedHourlyRecord.AvgBpm
                    || existingHourlyRecord.MinBpm != importedHourlyRecord.MinBpm
                    || existingHourlyRecord.MaxBpm != importedHourlyRecord.MaxBpm
                    || existingHourlyRecord.MeasurementCount != importedHourlyRecord.MeasurementCount)
                {
                    return false;
                }
            }

            return true;
        }

        private static void ApplyChanges(HeartRateDay existingHeartRateDay, HeartRateDay importedHeartRateDay)
        {
            existingHeartRateDay.StartTimeUtc = importedHeartRateDay.StartTimeUtc;
            existingHeartRateDay.EndTimeUtc = importedHeartRateDay.EndTimeUtc;
            existingHeartRateDay.AvgBpm = importedHeartRateDay.AvgBpm;
            existingHeartRateDay.MinBpm = importedHeartRateDay.MinBpm;
            existingHeartRateDay.MaxBpm = importedHeartRateDay.MaxBpm;
            existingHeartRateDay.MeasurementCount = importedHeartRateDay.MeasurementCount;
            existingHeartRateDay.HourlyRecords = importedHeartRateDay.HourlyRecords.ToList();
        }
    }
}