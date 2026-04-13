using Common.Dtos.DataReadDtos.Dashboard;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Mappers;

namespace Logic
{
    public class HomepageDashboardLogic : IHomepageDashboardLogic
    {
        private readonly IImportBatchDataAccess _importBatchDataAccess;
        private readonly IActivityDayDataAccess _activityDayDataAccess;

        public HomepageDashboardLogic(
            IImportBatchDataAccess importBatchDataAccess,
            IActivityDayDataAccess activityDayDataAccess)
        {
            _importBatchDataAccess = importBatchDataAccess;
            _activityDayDataAccess = activityDayDataAccess;
        }

        public HomepageDashboardDto GetHomepageDashboard()
        {
            var latestImportBatch = _importBatchDataAccess.GetLatestImportBatch();
            var latestActivityDay = _activityDayDataAccess.GetLatestActivityDay();

            return HomepageDashboardMapper.MapToHomepageDashboardDto(
                latestImportBatch,
                latestActivityDay);
        }
    }
}