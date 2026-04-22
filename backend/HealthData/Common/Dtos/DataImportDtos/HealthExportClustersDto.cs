namespace Common.Dtos.DataImportDtos
{
    public class HealthExportClustersDto
    {
        public ActivityClusterDto? Activity { get; set; }

        public SleepClusterDto? Sleep { get; set; }

        public VitalsClusterDto? Vitals { get; set; }
    }
}