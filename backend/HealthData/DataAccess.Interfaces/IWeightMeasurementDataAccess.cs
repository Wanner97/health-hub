using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IWeightMeasurementDataAccess
    {
        List<WeightMeasurement> GetWeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc);

        WeightMeasurement? GetLatestWeightMeasurement();
    }
}