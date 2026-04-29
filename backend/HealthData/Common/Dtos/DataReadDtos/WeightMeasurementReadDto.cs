namespace Common.Dtos.DataReadDtos
{
    public class WeightMeasurementReadDto
    {
        public DateOnly Date { get; set; }

        public double WeightKg { get; set; }

        public DateTime MeasuredAtUtc { get; set; }
    }
}