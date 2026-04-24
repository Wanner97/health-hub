namespace Common.Dtos.DataImportDtos
{
    public class BloodOxygenDayDto
    {
        public DateOnly Date { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public double AvgPercent { get; set; }

        public double MinPercent { get; set; }

        public double MaxPercent { get; set; }

        public int MeasurementCount { get; set; }
    }
}