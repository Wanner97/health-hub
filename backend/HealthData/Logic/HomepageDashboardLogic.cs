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
        private readonly ISleepSessionDataAccess _sleepSessionDataAccess;
        private readonly IHeartRateDayDataAccess _heartRateDayDataAccess;

        public HomepageDashboardLogic(
            IImportBatchDataAccess importBatchDataAccess,
            IActivityDayDataAccess activityDayDataAccess,
            ISleepSessionDataAccess sleepSessionDataAccess,
            IHeartRateDayDataAccess heartRateDayDataAccess)
        {
            _importBatchDataAccess = importBatchDataAccess;
            _activityDayDataAccess = activityDayDataAccess;
            _sleepSessionDataAccess = sleepSessionDataAccess;
            _heartRateDayDataAccess = heartRateDayDataAccess;
        }

        public HomepageDashboardDto GetHomepageDashboard()
        {
            var latestImportBatch = _importBatchDataAccess.GetLatestImportBatch();
            var latestActivityDay = _activityDayDataAccess.GetLatestActivityDay();
            var latestSleepSession = _sleepSessionDataAccess.GetLatestSleepSession();
            var latestHeartRateDay = _heartRateDayDataAccess.GetLatestHeartRateDay();

            return HomepageDashboardMapper.MapToHomepageDashboardDto(
                latestImportBatch,
                latestActivityDay,
                latestSleepSession,
                latestHeartRateDay);
        }
    }
}