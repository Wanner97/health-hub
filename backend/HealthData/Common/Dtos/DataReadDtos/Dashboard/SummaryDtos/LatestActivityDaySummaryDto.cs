namespace Common.Dtos.DataReadDtos.Dashboard.SummaryDtos
{
    public class LatestActivityDaySummaryDto
    {
        public DateOnly Date { get; set; }

        public int Steps { get; set; }

        public double DistanceMeters { get; set; }

        public string DistanceSource { get; set; } = string.Empty;

        public double TotalCaloriesBurnedKcal { get; set; }
    }
}