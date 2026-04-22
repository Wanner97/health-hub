using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IHeartRateDayDataAccess
    {
        List<HeartRateDay> GetHeartRateDays(DateOnly? from, DateOnly? to, bool includeHourlyRecords);

        HeartRateDay? GetLatestHeartRateDay();
    }
}