using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        ImportBatch CreateImportBatch(ImportBatch importBatch);
        bool ImportBatchExists(string source, int exportVersion, DateTimeOffset exportedAt);
    }
}
