namespace Common.Dtos.DataImportDtos
{
    public class HealthExportClustersDto
    {
        public ActivityClusterDto? Activity { get; set; }

        public SleepClusterDto? Sleep { get; set; }

        public VitalsClusterDto? Vitals { get; set; }

        public BodyClusterDto? Body { get; set; }

        public NutritionClusterDto? Nutrition { get; set; }
    }
}