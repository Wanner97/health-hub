using Common.Models;

namespace Logic.Helpers
{
    public class ActivityImportUpsertData
    {
        public List<ActivityDay> InsertedActivityDays { get; set; } = new();

        public List<ActivityDay> UpdatedActivityDays { get; set; } = new();

        public int UnchangedCount { get; set; }
    }
}