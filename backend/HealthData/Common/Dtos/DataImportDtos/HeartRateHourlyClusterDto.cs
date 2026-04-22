namespace Common.Dtos.DataImportDtos
{
    public class HeartRateHourlyClusterDto
    {
        public List<HeartRateHourlyRecordDto> Records { get; set; } = new();
    }
}