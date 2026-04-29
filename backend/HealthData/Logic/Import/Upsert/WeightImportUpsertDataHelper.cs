using Common.Import;
using Common.Models;

namespace Logic.Import.Upsert
{
    public static class WeightImportUpsertDataHelper
    {
        private const double WeightToleranceKg = 0.0001d;

        public static ImportUpsertData<WeightMeasurement> BuildUpsertData(
            List<WeightMeasurement> importedWeightMeasurements,
            Dictionary<DateTime, WeightMeasurement> existingWeightMeasurementsByMeasuredAtUtc)
        {
            var upsertData = new ImportUpsertData<WeightMeasurement>();

            foreach (var importedWeightMeasurement in importedWeightMeasurements.OrderBy(x => x.MeasuredAtUtc))
            {
                if (existingWeightMeasurementsByMeasuredAtUtc.TryGetValue(importedWeightMeasurement.MeasuredAtUtc, out var existingWeightMeasurement))
                {
                    if (HasChanges(existingWeightMeasurement, importedWeightMeasurement))
                    {
                        ApplyChanges(existingWeightMeasurement, importedWeightMeasurement);
                        upsertData.UpdatedItems.Add(existingWeightMeasurement);
                    }
                    else
                    {
                        upsertData.UnchangedCount++;
                    }
                }
                else
                {
                    upsertData.InsertedItems.Add(importedWeightMeasurement);
                }
            }

            return upsertData;
        }

        private static bool HasChanges(
            WeightMeasurement existingWeightMeasurement,
            WeightMeasurement importedWeightMeasurement)
        {
            return existingWeightMeasurement.Date != importedWeightMeasurement.Date
                || Math.Abs(existingWeightMeasurement.WeightKg - importedWeightMeasurement.WeightKg) > WeightToleranceKg;
        }

        private static void ApplyChanges(
            WeightMeasurement existingWeightMeasurement,
            WeightMeasurement importedWeightMeasurement)
        {
            existingWeightMeasurement.Date = importedWeightMeasurement.Date;
            existingWeightMeasurement.WeightKg = importedWeightMeasurement.WeightKg;
        }
    }
}