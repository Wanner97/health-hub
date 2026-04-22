using Common.Models;

namespace Logic.Helpers
{
    public class HeartRateImportUpsertData
    {
        public List<HeartRateDay> InsertedHeartRateDays { get; set; } = new();

        public List<HeartRateDay> UpdatedHeartRateDays { get; set; } = new();

        public int UnchangedCount { get; set; }
    }
}