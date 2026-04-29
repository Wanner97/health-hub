namespace Common.Dtos.DataReadDtos.Dashboard.SummaryDtos
{
    public class LatestWeightMeasurementSummaryDto
    {
        public DateOnly Date { get; set; }

        public double WeightKg { get; set; }

        public DateTime MeasuredAtUtc { get; set; }
    }
}