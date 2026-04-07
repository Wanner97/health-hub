using Common.Dtos;
using Common.Models;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Validators;
using System.Text.Json;
using FluentValidation;

namespace Logic
{
    public class StepsImportLogic : IStepsImportLogic
    {
        private readonly IImportBatchDataAccess _importBatchDataAccess;

        public StepsImportLogic(IImportBatchDataAccess importBatchDataAccess)
        {
            _importBatchDataAccess = importBatchDataAccess;
        }

        public ImportBatch ImportSteps(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var dto = JsonSerializer.Deserialize<StepsExportDto>(json);

            if (dto == null)
            {
                throw new InvalidOperationException("The JSON file could not be deserialized.");
            }

            var importBatch = new ImportBatch
            {
                Source = dto.Source,
                ExportVersion = dto.ExportVersion,
                ExportedAt = dto.ExportedAt,
                ImportedAt = DateTimeOffset.UtcNow,
                RecordCount = dto.Records.Count,
                StepEntries = dto.Records.Select(x => new StepEntry
                {
                    Date = x.Date,
                    Count = x.Count,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                }).ToList()
            };

            new ImportBatchValidator(false).ValidateAndThrow(importBatch);

            if (_importBatchDataAccess.ImportBatchExists(importBatch.Source, importBatch.ExportVersion, importBatch.ExportedAt))
            {
                throw new ValidationException("This import is already in the database.");
            }

            return _importBatchDataAccess.CreateImportBatch(importBatch);
        }
    }
}
