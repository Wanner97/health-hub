import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthLabel,
} from '../../utils/date/dateFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';

function formatPercent(value) {
  return `${formatNumber(Math.round(value ?? 0))} %`;
}

function BloodOxygenDaysTable({ rows, period }) {
  if (!rows?.length) {
    return (
      <section className="table-section">
        <h2>Blutsauerstoffübersicht</h2>
        <p>Es wurden keine Blutsauerstoffdaten gefunden.</p>
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
              <th>Ø %</th>
              <th>Min %</th>
              <th>Max %</th>
              <th>Messungen</th>
              <th>Tage</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((month) => (
              <tr key={month.monthKey}>
                <td>{formatMonthLabel(month.monthKey)}</td>
                <td>{formatPercent(month.averagePercent)}</td>
                <td>{formatPercent(month.minPercent)}</td>
                <td>{formatPercent(month.maxPercent)}</td>
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
      <h2>Blutsauerstoffübersicht</h2>

      <table>
        <thead>
          <tr>
            <th>Datum</th>
            <th>Ø %</th>
            <th>Min %</th>
            <th>Max %</th>
            <th>Messungen</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((day) => (
            <tr key={day.date}>
              <td>{formatDate(day.date)}</td>
              <td>{formatPercent(day.avgPercent)}</td>
              <td>{formatPercent(day.minPercent)}</td>
              <td>{formatPercent(day.maxPercent)}</td>
              <td>{formatNumber(day.measurementCount)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default BloodOxygenDaysTable;