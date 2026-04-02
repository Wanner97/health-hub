using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IStepEntryDataAccess
    {
        StepEntry CreateStepEntry(StepEntry stepEntry);
    }
}
