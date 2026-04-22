namespace Common.Dtos.DataImportDtos
{
    public class HeartRateDayDto
    {
        public DateOnly Date { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public int AvgBpm { get; set; }

        public int MinBpm { get; set; }

        public int MaxBpm { get; set; }

        public int MeasurementCount { get; set; }
    }
}