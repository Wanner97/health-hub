using Common.Dtos;

namespace Logic.Interfaces
{
    public interface IStepsReadLogic
    {
        LatestStepsImportDto GetLatestSteps();
    }
}