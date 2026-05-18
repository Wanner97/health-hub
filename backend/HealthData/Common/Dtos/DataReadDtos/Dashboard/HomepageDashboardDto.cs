using Common.Dtos.DataReadDtos.Dashboard.SummaryDtos;

namespace Common.Dtos.DataReadDtos.Dashboard
{
    public class HomepageDashboardDto
    {
        public LatestImportSummaryDto? LatestImport { get; set; }

        public LatestActivityDaySummaryDto? LatestActivityDay { get; set; }

        public LatestSleepSessionSummaryDto? LatestSleepSession { get; set; }

        public LatestHeartRateDaySummaryDto? LatestHeartRateDay { get; set; }

        public LatestBloodOxygenDaySummaryDto? LatestBloodOxygenDay { get; set; }

        public LatestHeightMeasurementSummaryDto? LatestHeightMeasurement { get; set; }

        public LatestWeightMeasurementSummaryDto? LatestWeightMeasurement { get; set; }

        public LatestNutritionDaySummaryDto? LatestNutritionDay { get; set; }
    }
}