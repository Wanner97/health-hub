import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatDateTimeUtc,
  formatMonthLabel,
} from '../../utils/date/dateFormatters';
import {
  formatNumber,
  formatWeightKg,
} from '../../utils/number/numberFormatters';

function WeightMeasurementsTable({ rows, period }) {
  if (!rows?.length) {
    return (
      <section className="table-section">
        <h2>Gewichtsübersicht</h2>
        <p>Es wurden keine Gewichtsdaten gefunden.</p>
      </section>
    );
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return (
      <section className="table-section">
        <h2>Monatsübersicht</h2>

        <table>
          <thead>
            <tr>
              <th>Monat</th>
              <th>Ø Gewicht</th>
              <th>Min Gewicht</th>
              <th>Max Gewicht</th>
              <th>Messungen</th>
              <th>Tage mit Messung</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((month) => (
              <tr key={month.monthKey}>
                <td>{formatMonthLabel(month.monthKey)}</td>
                <td>{formatWeightKg(month.averageWeightKg)}</td>
                <td>{formatWeightKg(month.minWeightKg)}</td>
                <td>{formatWeightKg(month.maxWeightKg)}</td>
                <td>{formatNumber(month.measurementCount)}</td>
                <td>{formatNumber(month.dayCount)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    );
  }

  return (
    <section className="table-section">
      <h2>Gewichtsübersicht</h2>

      <table>
        <thead>
          <tr>
            <th>Datum</th>
            <th>Gewicht</th>
            <th>Gemessen am (UTC)</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((measurement, index) => (
            <tr
              key={`${measurement.date}-${measurement.measuredAtUtc ?? index}`}
            >
              <td>{formatDate(measurement.date)}</td>
              <td>{formatWeightKg(measurement.weightKg)}</td>
              <td>{formatDateTimeUtc(measurement.measuredAtUtc)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default WeightMeasurementsTable;