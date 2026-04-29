import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatDateTimeUtc,
  formatMonthLabel,
} from '../../utils/date/dateFormatters';
import {
  formatCaloriesKcal,
  formatKilometersFromMeters,
  formatNumber,
} from '../../utils/number/numberFormatters';

function ActivityDaysTable({ rows, period }) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return (
      <section className="table-section">
        <h2>Monatsübersicht</h2>

        <table>
          <thead>
            <tr>
              <th>Monat</th>
              <th>Ø Schritte / Tag</th>
              <th>Ø km / Tag</th>
              <th>Ø kcal / Tag</th>
              <th>Schritte gesamt</th>
              <th>Tage</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((month) => (
              <tr key={month.monthKey}>
                <td>{formatMonthLabel(month.monthKey)}</td>
                <td>{formatNumber(month.averageSteps)}</td>
                <td>{formatKilometersFromMeters(month.averageDistanceMeters)}</td>
                <td>{formatCaloriesKcal(month.averageCaloriesBurnedKcal)}</td>
                <td>{formatNumber(month.totalSteps)}</td>
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
      <h2>Aktivitätsübersicht</h2>

      <table>
        <thead>
          <tr>
            <th>Datum</th>
            <th>Schritte</th>
            <th>Distanz (km)</th>
            <th>Kalorien</th>
            <th>Start (UTC)</th>
            <th>Ende (UTC)</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((day) => (
            <tr key={day.date}>
              <td>{formatDate(day.date)}</td>
              <td>{formatNumber(day.steps)}</td>
              <td>{formatKilometersFromMeters(day.distanceMeters)}</td>
              <td>{formatCaloriesKcal(day.totalCaloriesBurnedKcal)}</td>
              <td>{formatDateTimeUtc(day.startTimeUtc)}</td>
              <td>{formatDateTimeUtc(day.endTimeUtc)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default ActivityDaysTable;