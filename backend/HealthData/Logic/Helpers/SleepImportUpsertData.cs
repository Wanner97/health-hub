using Common.Models;

namespace Logic.Helpers
{
    public class SleepImportUpsertData
    {
        public List<SleepSession> InsertedSleepSessions { get; set; } = new();

        public List<SleepSession> UpdatedSleepSessions { get; set; } = new();

        public int UnchangedCount { get; set; }
    }
}