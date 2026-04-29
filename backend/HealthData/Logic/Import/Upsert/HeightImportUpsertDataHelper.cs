using Common.Import;
using Common.Models;

namespace Logic.Import.Upsert
{
    public static class HeightImportUpsertDataHelper
    {
        private const double HeightToleranceCm = 0.0001d;

        public static ImportUpsertData<HeightMeasurement> BuildUpsertData(
            List<HeightMeasurement> importedHeightMeasurements,
            Dictionary<DateTime, HeightMeasurement> existingHeightMeasurementsByMeasuredAtUtc)
        {
            var upsertData = new ImportUpsertData<HeightMeasurement>();

            foreach (var importedHeightMeasurement in importedHeightMeasurements.OrderBy(x => x.MeasuredAtUtc))
            {
                if (existingHeightMeasurementsByMeasuredAtUtc.TryGetValue(importedHeightMeasurement.MeasuredAtUtc, out var existingHeightMeasurement))
                {
                    if (HasChanges(existingHeightMeasurement, importedHeightMeasurement))
                    {
                        ApplyChanges(existingHeightMeasurement, importedHeightMeasurement);
                        upsertData.UpdatedItems.Add(existingHeightMeasurement);
                    }
                    else
                    {
                        upsertData.UnchangedCount++;
                    }
                }
                else
                {
                    upsertData.InsertedItems.Add(importedHeightMeasurement);
                }
            }

            return upsertData;
        }

        private static bool HasChanges(
            HeightMeasurement existingHeightMeasurement,
            HeightMeasurement importedHeightMeasurement)
        {
            return Math.Abs(existingHeightMeasurement.HeightCm - importedHeightMeasurement.HeightCm) > HeightToleranceCm;
        }

        private static void ApplyChanges(
            HeightMeasurement existingHeightMeasurement,
            HeightMeasurement importedHeightMeasurement)
        {
            existingHeightMeasurement.HeightCm = importedHeightMeasurement.HeightCm;
        }
    }
}