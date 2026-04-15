using Common.Models;

namespace DataAccess.Interfaces
{
    public interface ISleepSessionDataAccess
    {
        List<SleepSession> GetSleepSessions(DateOnly? from, DateOnly? to);
        SleepSession? GetLatestSleepSession();
    }
}