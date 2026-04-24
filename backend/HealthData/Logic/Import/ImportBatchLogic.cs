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
        private readonly IVersionManifestProvider _versionManifestProvider;

        public ImportBatchLogic(
            IImportBatchDataAccess importBatchDataAccess,
            IVersionManifestProvider versionManifestProvider)
        {
            _importBatchDataAccess = importBatchDataAccess;
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

        private HealthImportUpsertData BuildHealthImportUpsertData(HealthImportData importData)
        {
            var existingActivityDaysByDate = _importBatchDataAccess.GetExistingActivityDays(importData.ImportBatch.Source, importData.ActivityDays.Select(x => x.Date));

            var existingSleepSessionsByStartTime = _importBatchDataAccess.GetExistingSleepSessions(importData.ImportBatch.Source, importData.SleepSessions.Select(x => x.StartTimeUtc));

            var existingHeartRateDaysByDate = _importBatchDataAccess.GetExistingHeartRateDays(importData.ImportBatch.Source, importData.HeartRateDays.Select(x => x.Date));

            var existingBloodOxygenDaysByDate = _importBatchDataAccess.GetExistingBloodOxygenDays(importData.ImportBatch.Source, importData.BloodOxygenDays.Select(x => x.Date));

            return new HealthImportUpsertData
            {
                ActivityDays = ActivityImportUpsertDataHelper.BuildUpsertData(importData.ActivityDays, existingActivityDaysByDate),

                SleepSessions = SleepImportUpsertDataHelper.BuildUpsertData(importData.SleepSessions, existingSleepSessionsByStartTime),

                HeartRateDays = HeartRateImportUpsertDataHelper.BuildUpsertData(importData.HeartRateDays, existingHeartRateDaysByDate),

                BloodOxygenDays = BloodOxygenImportUpsertDataHelper.BuildUpsertData(importData.BloodOxygenDays, existingBloodOxygenDaysByDate)
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