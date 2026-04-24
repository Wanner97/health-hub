using Common.Models;

namespace Logic.Helpers
{
    public class BloodOxygenImportUpsertData
    {
        public List<BloodOxygenDay> InsertedBloodOxygenDays { get; set; } = new();

        public List<BloodOxygenDay> UpdatedBloodOxygenDays { get; set; } = new();

        public int UnchangedCount { get; set; }
    }
}