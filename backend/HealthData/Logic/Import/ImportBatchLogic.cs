using Common.Dtos.DataReadDtos;
using Common.Import;
using Common.Models;
using Common.Versioning;
using DataAccess.Interfaces;
using FluentValidation;
using Logic.Import.Helpers;
using Logic.Import.Upsert;
using Logic.Interfaces;
using Logic.Read.Mappers;
using Logic.Validators;

namespace Logic.Import
{
    public class ImportBatchLogic : IImportBatchLogic
    {
        private readonly IImportBatchDataAccess _importBatchDataAccess;
        private readonly IHeightMeasurementDataAccess _heightMeasurementDataAccess;
        private readonly IVersionManifestProvider _versionManifestProvider;
        private readonly IHealthImportExistingDataLoader _healthImportExistingDataLoader;

        public ImportBatchLogic(
            IImportBatchDataAccess importBatchDataAccess,
            IHeightMeasurementDataAccess heightMeasurementDataAccess,
            IHealthImportExistingDataLoader healthImportExistingDataLoader,
            IVersionManifestProvider versionManifestProvider)
        {
            _importBatchDataAccess = importBatchDataAccess;
            _heightMeasurementDataAccess = heightMeasurementDataAccess;
            _healthImportExistingDataLoader = healthImportExistingDataLoader;
            _versionManifestProvider = versionManifestProvider;
        }

        public List<ImportBatchReadDto> GetImportBatches(DateOnly? from, DateOnly? to)
        {
            var importBatches = _importBatchDataAccess.GetImportBatches(from, to);

            return ImportBatchReadMapper.MapToReadDtos(importBatches);
        }

        public ImportBatch ImportHealthData(Stream stream)
        {
            var dto = HealthImportHelper.DeserializeHealthExport(stream);
            var importedAtUtc = DateTime.UtcNow;

            var importData = HealthImportHelper.BuildHealthImportData(dto, importedAtUtc);

            var bodyHeightMetersForDistanceCalculation = ResolveBodyHeightMetersForDistanceCalculation(importData);

            ActivityDistanceCalculationHelper.FillMissingDistances(
                importData.ActivityDays,
                bodyHeightMetersForDistanceCalculation);

            ValidateImportBatch(importData.ImportBatch);

            var upsertData = BuildHealthImportUpsertData(importData);

            ApplyImportCounts(importData.ImportBatch, upsertData);

            HealthImportHelper.ClearImportBatchNavigationEntries(importData.ImportBatch);

            return _importBatchDataAccess.ApplyImport(importData.ImportBatch, upsertData);
        }

        private void ValidateImportBatch(ImportBatch importBatch)
        {
            var expectedExportVersion = _versionManifestProvider.AndroidVersion;

            new ImportBatchValidator(false, expectedExportVersion).ValidateAndThrow(importBatch);
        }

        private double? ResolveBodyHeightMetersForDistanceCalculation(HealthImportData importData)
        {
            var latestImportedHeightMeters = importData.HeightMeasurements
                .OrderByDescending(x => x.MeasuredAtUtc)
                .Select(x => (double?)(x.HeightCm / 100.0))
                .FirstOrDefault();

            if (latestImportedHeightMeters.HasValue)
            {
                return latestImportedHeightMeters.Value;
            }

            var latestStoredHeightMeasurement = _heightMeasurementDataAccess.GetLatestHeightMeasurement();

            if (latestStoredHeightMeasurement != null && latestStoredHeightMeasurement.HeightCm > 0)
            {
                return latestStoredHeightMeasurement.HeightCm / 100.0;
            }

            return null;
        }

        private HealthImportUpsertData BuildHealthImportUpsertData(HealthImportData importData)
        {
            var existingActivityDaysByDate = _healthImportExistingDataLoader.GetExistingActivityDays(
                importData.ImportBatch.Source,
                importData.ActivityDays.Select(x => x.Date));

            var existingSleepSessionsByStartTime = _healthImportExistingDataLoader.GetExistingSleepSessions(
                importData.ImportBatch.Source,
                importData.SleepSessions.Select(x => x.StartTimeUtc));

            var existingHeartRateDaysByDate = _healthImportExistingDataLoader.GetExistingHeartRateDays(
                importData.ImportBatch.Source,
                importData.HeartRateDays.Select(x => x.Date));

            var existingBloodOxygenDaysByDate = _healthImportExistingDataLoader.GetExistingBloodOxygenDays(
                importData.ImportBatch.Source,
                importData.BloodOxygenDays.Select(x => x.Date));

            var existingHeightMeasurementsByMeasuredAtUtc = _healthImportExistingDataLoader.GetExistingHeightMeasurements(
                importData.ImportBatch.Source,
                importData.HeightMeasurements.Select(x => x.MeasuredAtUtc));

            var existingWeightMeasurementsByMeasuredAtUtc = _healthImportExistingDataLoader.GetExistingWeightMeasurements(
                importData.ImportBatch.Source,
                importData.WeightMeasurements.Select(x => x.MeasuredAtUtc));

            var existingNutritionRecordsByHealthConnectRecordId = _healthImportExistingDataLoader.GetExistingNutritionRecords(
                importData.ImportBatch.Source,
                importData.NutritionRecords.Select(x => x.HealthConnectRecordId));

            return new HealthImportUpsertData
            {
                ActivityDays = ActivityImportUpsertDataHelper.BuildUpsertData(
                    importData.ActivityDays,
                    existingActivityDaysByDate),

                SleepSessions = SleepImportUpsertDataHelper.BuildUpsertData(
                    importData.SleepSessions,
                    existingSleepSessionsByStartTime),

                HeartRateDays = HeartRateImportUpsertDataHelper.BuildUpsertData(
                    importData.HeartRateDays,
                    existingHeartRateDaysByDate),

                BloodOxygenDays = BloodOxygenImportUpsertDataHelper.BuildUpsertData(
                    importData.BloodOxygenDays,
                    existingBloodOxygenDaysByDate),

                HeightMeasurements = HeightImportUpsertDataHelper.BuildUpsertData(
                    importData.HeightMeasurements,
                    existingHeightMeasurementsByMeasuredAtUtc),

                WeightMeasurements = WeightImportUpsertDataHelper.BuildUpsertData(
                    importData.WeightMeasurements,
                    existingWeightMeasurementsByMeasuredAtUtc),

                NutritionRecords = NutritionImportUpsertDataHelper.BuildUpsertData(
                    importData.NutritionRecords,
                    existingNutritionRecordsByHealthConnectRecordId)
            };
        }

        private static void ApplyImportCounts(ImportBatch importBatch, HealthImportUpsertData upsertData)
        {
            importBatch.InsertedRecordCount = ImportBatchRecordCountHelper.CalculateInsertedRecordCount(upsertData);
            importBatch.UpdatedRecordCount = ImportBatchRecordCountHelper.CalculateUpdatedRecordCount(upsertData);
            importBatch.UnchangedRecordCount = ImportBatchRecordCountHelper.CalculateUnchangedRecordCount(upsertData);
        }
    }
}