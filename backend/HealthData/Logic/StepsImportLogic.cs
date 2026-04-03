using Common.Dtos;
using Common.Models;
using DataAccess.Interfaces;
using Logic.Interfaces;
using System.Text.Json;

namespace Logic
{
    public class StepsImportLogic : IStepsImportLogic
    {
        private readonly IImportBatchDataAccess _importBatchDataAccess;

        public StepsImportLogic(IImportBatchDataAccess importBatchDataAccess)
        {
            _importBatchDataAccess = importBatchDataAccess;
        }

        public void ImportSteps(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var dto = JsonSerializer.Deserialize<StepsExportDto>(json);

            if (dto == null)
            {
                throw new InvalidOperationException("Die JSON-Datei konnte nicht deserialisiert werden.");
            }

            Validate(dto);

            var importBatch = new StepRecordsImportBatch
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

            _importBatchDataAccess.CreateImportBatch(importBatch);
        }

        private static void Validate(StepsExportDto dto)
        {
            if (dto.ExportVersion != 1)
            {
                throw new InvalidOperationException("ExportVersion wird nicht unterstützt.");
            }

            if (string.IsNullOrWhiteSpace(dto.Source))
            {
                throw new InvalidOperationException("Source darf nicht leer sein.");
            }

            if (dto.Records == null || dto.Records.Count == 0)
            {
                throw new InvalidOperationException("Es wurden keine Step-Records gefunden.");
            }

            foreach (var record in dto.Records)
            {
                if (record.Count < 0)
                {
                    throw new InvalidOperationException("Count darf nicht negativ sein.");
                }

                if (record.StartTime >= record.EndTime)
                {
                    throw new InvalidOperationException("StartTime muss vor EndTime liegen.");
                }
            }

            var duplicateDates = dto.Records
                .GroupBy(x => x.Date)
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateDates.Count > 0)
            {
                throw new InvalidOperationException("Die Datei enthält doppelte Datumswerte.");
            }
        }
    }
}
