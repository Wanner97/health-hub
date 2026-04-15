using Common.Dtos.DataImportDtos;
using Common.Dtos.DataReadDtos;
using Common.Models;
using DataAccess.Interfaces;
using FluentValidation;
using Logic.Helpers;
using Logic.Interfaces;
using Logic.Mappers;
using Logic.Validators;
using System.Text.Json;

namespace Logic
{
    public class ImportBatchLogic : IImportBatchLogic
    {
        private readonly IImportBatchDataAccess _importBatchDataAccess;

        public ImportBatchLogic(IImportBatchDataAccess importBatchDataAccess)
        {
            _importBatchDataAccess = importBatchDataAccess;
        }

        public List<ImportBatchReadDto> GetImportBatches(DateOnly? from, DateOnly? to)
        {
            var importBatches = _importBatchDataAccess.GetImportBatches(from, to);

            return ImportBatchReadMapper.MapToReadDtos(importBatches);
        }

        public ImportBatch ImportHealthData(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();

                var dto = JsonSerializer.Deserialize<HealthExportDto>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (dto == null)
                {
                    throw new InvalidOperationException("The JSON file could not be deserialized.");
                }

                var importedAtUtc = DateTime.UtcNow;

                var importedActivityDays = ActivityImportMapper.MapToActivityDays(
                    dto.Source,
                    dto.Clusters.Activity);

                var consolidatedSleepSessionDtos = SleepSessionConsolidationHelper.ConsolidateSleepSessions(
                    dto.Clusters.Sleep?.Sessions ?? new List<SleepSessionDto>());

                var importedSleepSessions = SleepImportMapper.MapToSleepSessions(
                    dto.Source,
                    importedAtUtc,
                    consolidatedSleepSessionDtos);

                var importBatch = ImportBatchImportMapper.MapToImportBatch(
                    dto,
                    importedAtUtc,
                    importedActivityDays,
                    importedSleepSessions);

                new ImportBatchValidator(false).ValidateAndThrow(importBatch);

                var existingActivityDaysByDate = _importBatchDataAccess.GetExistingActivityDays(
                    importBatch.Source,
                    importedActivityDays.Select(x => x.Date));

                var activityUpsertData = ActivityImportUpsertDataHelper.BuildUpsertData(
                    importedActivityDays,
                    existingActivityDaysByDate);

                var existingSleepSessionsByStartTime = _importBatchDataAccess.GetExistingSleepSessions(
                    importBatch.Source,
                    importedSleepSessions.Select(x => x.StartTimeUtc));

                var sleepUpsertData = SleepImportUpsertDataHelper.BuildUpsertData(
                    importedSleepSessions,
                    existingSleepSessionsByStartTime);

                importBatch.InsertedRecordCount =
                    activityUpsertData.InsertedActivityDays.Count
                    + sleepUpsertData.InsertedSleepSessions.Count;

                importBatch.UpdatedRecordCount =
                    activityUpsertData.UpdatedActivityDays.Count
                    + sleepUpsertData.UpdatedSleepSessions.Count;

                importBatch.UnchangedRecordCount =
                    activityUpsertData.UnchangedCount
                    + sleepUpsertData.UnchangedCount;

                importBatch.ActivityDayEntries = new List<ActivityDay>();
                importBatch.SleepSessionEntries = new List<SleepSession>();

                return _importBatchDataAccess.ApplyImport(
                    importBatch,
                    activityUpsertData.InsertedActivityDays,
                    activityUpsertData.UpdatedActivityDays,
                    sleepUpsertData.InsertedSleepSessions,
                    sleepUpsertData.UpdatedSleepSessions);
            }
        }
    }
}