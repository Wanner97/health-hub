using Common.Models;

namespace Logic.Interfaces
{
    public interface IActivityImportLogic
    {
        ImportBatch ImportActivity(Stream stream);
    }
}
