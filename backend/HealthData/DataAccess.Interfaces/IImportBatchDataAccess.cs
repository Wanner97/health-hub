using Common.Import;
using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        ImportBatch? GetLatestImportBatch();

        List<ImportBatch> GetImportBatches(DateOnly? from, DateOnly? to);

        ImportBatch ApplyImport(ImportBatch importBatch, HealthImportUpsertData upsertData);
    }
}