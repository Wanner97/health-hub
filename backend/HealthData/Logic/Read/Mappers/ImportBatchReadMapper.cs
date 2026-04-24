using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Read.Mappers
{
    public static class ImportBatchReadMapper
    {
        public static ImportBatchReadDto MapToReadDto(ImportBatch importBatch)
        {
            return new ImportBatchReadDto
            {
                ExportedAtUtc = importBatch.ExportedAtUtc,
                ImportedAtUtc = importBatch.ImportedAtUtc,
                RangeStartUtc = importBatch.RangeStartUtc,
                RangeEndUtc = importBatch.RangeEndUtc,
                ReceivedRecordCount = importBatch.ReceivedRecordCount,
                InsertedRecordCount = importBatch.InsertedRecordCount,
                UpdatedRecordCount = importBatch.UpdatedRecordCount,
                UnchangedRecordCount = importBatch.UnchangedRecordCount,
                ExportVersion = importBatch.ExportVersion
            };
        }

        public static List<ImportBatchReadDto> MapToReadDtos(List<ImportBatch> importBatches)
        {
            return importBatches.Select(MapToReadDto).ToList();
        }
    }
}