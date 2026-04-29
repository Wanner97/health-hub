using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IHeightMeasurementDataAccess
    {
        List<HeightMeasurement> GetHeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc);

        HeightMeasurement? GetLatestHeightMeasurement();
    }
}