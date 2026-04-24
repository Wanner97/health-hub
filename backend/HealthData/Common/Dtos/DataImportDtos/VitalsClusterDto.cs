namespace Common.Dtos.DataImportDtos
{
    public class VitalsClusterDto
    {
        public HeartRateDailyClusterDto? HeartRateDaily { get; set; }

        public HeartRateHourlyClusterDto? HeartRateHourly { get; set; }

        public BloodOxygenDailyClusterDto? BloodOxygenDaily { get; set; }
    }
}