using Common.Models;

namespace Logic.Interfaces
{
    public interface IStepsImportLogic
    {
        ImportBatch ImportSteps(Stream stream);
    }
}
