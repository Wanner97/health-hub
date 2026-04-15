using Common.Models;

namespace Logic.Helpers
{
    public static class SleepImportUpsertDataHelper
    {
        public static SleepImportUpsertData BuildUpsertData(
            List<SleepSession> importedSleepSessions,
            Dictionary<DateTime, SleepSession> existingSleepSessionsByStartTime)
        {
            var upsertData = new SleepImportUpsertData();

            foreach (var importedSleepSession in importedSleepSessions.OrderBy(x => x.StartTimeUtc))
            {
                if (existingSleepSessionsByStartTime.TryGetValue(importedSleepSession.StartTimeUtc, out var existingSleepSession))
                {
                    if (HasChanges(existingSleepSession, importedSleepSession))
                    {
                        ApplyChanges(existingSleepSession, importedSleepSession);
                        upsertData.UpdatedSleepSessions.Add(existingSleepSession);
                    }
                    else
                    {
                        upsertData.UnchangedCount++;
                    }
                }
                else
                {
                    upsertData.InsertedSleepSessions.Add(importedSleepSession);
                }
            }

            return upsertData;
        }

        private static bool HasChanges(SleepSession existingSleepSession, SleepSession importedSleepSession)
        {
            return existingSleepSession.EndTimeUtc != importedSleepSession.EndTimeUtc
                   || existingSleepSession.DurationMinutes != importedSleepSession.DurationMinutes
                   || !HaveSameStages(existingSleepSession.SleepStages, importedSleepSession.SleepStages);
        }

        private static bool HaveSameStages(
            ICollection<SleepStage> existingSleepStages,
            ICollection<SleepStage> importedSleepStages)
        {
            var normalizedExistingSleepStages = existingSleepStages
                .OrderBy(x => x.StartTimeUtc)
                .ThenBy(x => x.EndTimeUtc)
                .ThenBy(x => x.Stage)
                .ToList();

            var normalizedImportedSleepStages = importedSleepStages
                .OrderBy(x => x.StartTimeUtc)
                .ThenBy(x => x.EndTimeUtc)
                .ThenBy(x => x.Stage)
                .ToList();

            if (normalizedExistingSleepStages.Count != normalizedImportedSleepStages.Count)
            {
                return false;
            }

            for (var i = 0; i < normalizedExistingSleepStages.Count; i++)
            {
                var existingSleepStage = normalizedExistingSleepStages[i];
                var importedSleepStage = normalizedImportedSleepStages[i];

                if (existingSleepStage.Stage != importedSleepStage.Stage
                    || existingSleepStage.StartTimeUtc != importedSleepStage.StartTimeUtc
                    || existingSleepStage.EndTimeUtc != importedSleepStage.EndTimeUtc)
                {
                    return false;
                }
            }

            return true;
        }

        private static void ApplyChanges(SleepSession existingSleepSession, SleepSession importedSleepSession)
        {
            existingSleepSession.EndTimeUtc = importedSleepSession.EndTimeUtc;
            existingSleepSession.DurationMinutes = importedSleepSession.DurationMinutes;

            existingSleepSession.SleepStages = importedSleepSession.SleepStages
                .Select(x => new SleepStage
                {
                    Stage = x.Stage,
                    StartTimeUtc = x.StartTimeUtc,
                    EndTimeUtc = x.EndTimeUtc
                })
                .ToList();
        }
    }
}