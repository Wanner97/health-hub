using Common.Dtos.DataImportDtos;
using Common.Models;
using DataAccess.Interfaces;
using FluentValidation;
using Logic.Interfaces;
using Logic.Mappers;
using Logic.Validators;
using System.Text.Json;
using Common.Dtos.DataReadDtos;
using Logic.Helpers;

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

        public ImportBatch ImportActivity(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();

                var dto = JsonSerializer.Deserialize<ActivityExportDto>(json);

                if (dto == null)
                {
                    throw new InvalidOperationException("The JSON file could not be deserialized.");
                }

                var importedAtUtc = DateTime.UtcNow;

                var importedActivityDays = ActivityImportMapper.MapToActivityDays(dto);

                var importBatch = ActivityImportMapper.MapToImportBatch(dto, importedAtUtc, importedActivityDays);

                new ImportBatchValidator(false).ValidateAndThrow(importBatch);

                var existingActivityDaysByDate = _importBatchDataAccess.GetExistingActivityDays(
                    importBatch.Source,
                    importedActivityDays.Select(x => x.Date));

                var upsertData = ActivityImportUpsertDataHelper.BuildUpsertData(
                    importedActivityDays,
                    existingActivityDaysByDate);

                importBatch.InsertedRecordCount = upsertData.InsertedActivityDays.Count;
                importBatch.UpdatedRecordCount = upsertData.UpdatedActivityDays.Count;
                importBatch.UnchangedRecordCount = upsertData.UnchangedCount;

                importBatch.ActivityDayEntries = new List<ActivityDay>();

                return _importBatchDataAccess.ApplyImport(
                    importBatch,
                    upsertData.InsertedActivityDays,
                    upsertData.UpdatedActivityDays);
            }
        }
    }
}