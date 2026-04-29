namespace Common.Dtos.DataImportDtos
{
    public class WeightRecordDto
    {
        public DateOnly Date { get; set; }

        public double WeightKg { get; set; }

        public DateTimeOffset MeasuredAt { get; set; }
    }
}