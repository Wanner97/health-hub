using Common.Dtos.DataReadDtos.Dashboard;
using DataAccess.Interfaces;
using Logic.Dashboard.Mappers;
using Logic.Interfaces;

namespace Logic.Dashboard
{
    public class HomepageDashboardLogic : IHomepageDashboardLogic
    {
        private readonly IDashboardDataAccess _dashboardDataAccess;

        public HomepageDashboardLogic(IDashboardDataAccess dashboardDataAccess)
        {
            _dashboardDataAccess = dashboardDataAccess;
        }

        public HomepageDashboardDto GetHomepageDashboard()
        {
            var dashboardData = _dashboardDataAccess.GetHomepageDashboardData();

            return HomepageDashboardMapper.MapToHomepageDashboardDto(dashboardData);
        }
    }
}