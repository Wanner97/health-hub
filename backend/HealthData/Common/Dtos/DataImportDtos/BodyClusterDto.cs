namespace Common.Dtos.DataImportDtos
{
    public class BodyClusterDto
    {
        public HeightRecordDto? LatestHeight { get; set; }

        public List<WeightRecordDto> WeightRecords { get; set; } = new();
    }
}