using Common.Models;

namespace Common.Dashboard
{
    public class HomepageDashboardData
    {
        public ImportBatch? LatestImportBatch { get; set; }

        public ActivityDay? LatestActivityDay { get; set; }

        public SleepSession? LatestSleepSession { get; set; }

        public HeartRateDay? LatestHeartRateDay { get; set; }

        public BloodOxygenDay? LatestBloodOxygenDay { get; set; }

        public HeightMeasurement? LatestHeightMeasurement { get; set; }

        public WeightMeasurement? LatestWeightMeasurement { get; set; }
    }
}