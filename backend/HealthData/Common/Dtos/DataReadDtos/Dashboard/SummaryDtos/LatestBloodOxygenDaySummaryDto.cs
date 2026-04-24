namespace Common.Dtos.DataReadDtos.Dashboard.SummaryDtos
{
    public class LatestBloodOxygenDaySummaryDto
    {
        public DateOnly Date { get; set; }

        public double AvgPercent { get; set; }

        public double MinPercent { get; set; }

        public double MaxPercent { get; set; }

        public int MeasurementCount { get; set; }
    }
}