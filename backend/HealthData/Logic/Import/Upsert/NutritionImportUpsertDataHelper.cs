using Common.Import;
using Common.Models;

namespace Logic.Import.Upsert
{
    public static class NutritionImportUpsertDataHelper
    {
        private const double AmountTolerance = 0.0001d;

        public static ImportUpsertData<NutritionRecord> BuildUpsertData(
            List<NutritionRecord> importedNutritionRecords,
            Dictionary<string, NutritionRecord> existingNutritionRecordsByHealthConnectRecordId)
        {
            var upsertData = new ImportUpsertData<NutritionRecord>();

            foreach (var importedNutritionRecord in importedNutritionRecords.OrderBy(x => x.Date).ThenBy(x => x.StartTimeUtc))
            {
                if (existingNutritionRecordsByHealthConnectRecordId.TryGetValue(importedNutritionRecord.HealthConnectRecordId, out var existingNutritionRecord))
                {
                    if (HasChanges(existingNutritionRecord, importedNutritionRecord))
                    {
                        ApplyChanges(existingNutritionRecord, importedNutritionRecord);
                        upsertData.UpdatedItems.Add(existingNutritionRecord);
                    }
                    else
                    {
                        upsertData.UnchangedCount++;
                    }
                }
                else
                {
                    upsertData.InsertedItems.Add(importedNutritionRecord);
                }
            }

            return upsertData;
        }

        private static bool HasChanges(NutritionRecord existingNutritionRecord, NutritionRecord importedNutritionRecord)
        {
            return existingNutritionRecord.Date != importedNutritionRecord.Date
                   || existingNutritionRecord.StartTimeUtc != importedNutritionRecord.StartTimeUtc
                   || existingNutritionRecord.EndTimeUtc != importedNutritionRecord.EndTimeUtc
                   || existingNutritionRecord.MealType != importedNutritionRecord.MealType
                   || existingNutritionRecord.Name != importedNutritionRecord.Name
                   || !HaveSameNutrients(existingNutritionRecord.Nutrients, importedNutritionRecord.Nutrients);
        }

        private static bool HaveSameNutrients(ICollection<NutritionRecordNutrient> existingNutrients, ICollection<NutritionRecordNutrient> importedNutrients)
        {
            var normalizedExistingNutrients = existingNutrients
                .OrderBy(x => x.NutrientKey)
                .ToList();

            var normalizedImportedNutrients = importedNutrients
                .OrderBy(x => x.NutrientKey)
                .ToList();

            if (normalizedExistingNutrients.Count != normalizedImportedNutrients.Count)
            {
                return false;
            }

            for (var i = 0; i < normalizedExistingNutrients.Count; i++)
            {
                var existingNutrient = normalizedExistingNutrients[i];
                var importedNutrient = normalizedImportedNutrients[i];

                if (existingNutrient.NutrientKey != importedNutrient.NutrientKey
                    || existingNutrient.Unit != importedNutrient.Unit
                    || Math.Abs(existingNutrient.Amount - importedNutrient.Amount) > AmountTolerance)
                {
                    return false;
                }
            }

            return true;
        }

        private static void ApplyChanges(NutritionRecord existingNutritionRecord, NutritionRecord importedNutritionRecord)
        {
            existingNutritionRecord.Date = importedNutritionRecord.Date;
            existingNutritionRecord.StartTimeUtc = importedNutritionRecord.StartTimeUtc;
            existingNutritionRecord.EndTimeUtc = importedNutritionRecord.EndTimeUtc;
            existingNutritionRecord.MealType = importedNutritionRecord.MealType;
            existingNutritionRecord.Name = importedNutritionRecord.Name;

            existingNutritionRecord.Nutrients = importedNutritionRecord.Nutrients
                .Select(x => new NutritionRecordNutrient
                {
                    NutrientKey = x.NutrientKey,
                    Amount = x.Amount,
                    Unit = x.Unit
                })
                .ToList();
        }
    }
}