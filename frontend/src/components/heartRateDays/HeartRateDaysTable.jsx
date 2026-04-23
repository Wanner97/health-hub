import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthLabel,
} from '../../utils/date/dateFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';

function HeartRateDaysTable({ rows, period }) {
  if (!rows?.length) {
    return (
      <section className="table-section">
        <h2>Herzfrequenzübersicht</h2>
        <p>Es wurden keine Herzfrequenzdaten gefunden.</p>
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
              <th>Ø BPM</th>
              <th>Min BPM</th>
              <th>Max BPM</th>
              <th>Messungen</th>
              <th>Tage</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((month) => (
              <tr key={month.monthKey}>
                <td>{formatMonthLabel(month.monthKey)}</td>
                <td>{formatNumber(month.averageBpm)}</td>
                <td>{formatNumber(month.minBpm)}</td>
                <td>{formatNumber(month.maxBpm)}</td>
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
      <h2>Herzfrequenzübersicht</h2>

      <table>
        <thead>
          <tr>
            <th>Datum</th>
            <th>Ø BPM</th>
            <th>Min BPM</th>
            <th>Max BPM</th>
            <th>Messungen</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((day) => (
            <tr key={day.date}>
              <td>{formatDate(day.date)}</td>
              <td>{formatNumber(day.avgBpm)}</td>
              <td>{formatNumber(day.minBpm)}</td>
              <td>{formatNumber(day.maxBpm)}</td>
              <td>{formatNumber(day.measurementCount)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default HeartRateDaysTable;