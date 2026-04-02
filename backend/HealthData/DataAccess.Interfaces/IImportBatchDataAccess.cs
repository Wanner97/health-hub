using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        ImportBatch CreateImportBatch(ImportBatch importBatch);
    }
}
