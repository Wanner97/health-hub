namespace Common.Dtos.DataReadDtos.Dashboard.SummaryDtos
{
    public class LatestHeartRateDaySummaryDto
    {
        public DateOnly Date { get; set; }

        public int AvgBpm { get; set; }

        public int MinBpm { get; set; }

        public int MaxBpm { get; set; }

        public int MeasurementCount { get; set; }
    }
}