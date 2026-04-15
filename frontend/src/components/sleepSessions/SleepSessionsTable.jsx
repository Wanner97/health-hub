import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthLabel,
} from '../../utils/date/dateFormatters';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';

function formatSleepDateRange(sleepDateStartKey, sleepDateEndKey) {
  const startDate = formatDate(sleepDateStartKey);
  const endDate = formatDate(sleepDateEndKey);

  return `${startDate} – ${endDate}`;
}

function formatPercentage(value) {
  if (value == null) {
    return '-';
  }

  return `${Math.round(value)}%`;
}

function SleepSessionsTable({ rows, period }) {
  if (!rows?.length) {
    return (
      <section className="table-section">
        <h2>Schlafübersicht</h2>
        <p>Es wurden keine Schlafdaten gefunden.</p>
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
              <th>Ø Schlaf / Tag</th>
              <th>Schlaf gesamt</th>
              <th>Sessions</th>
              <th>Wach</th>
              <th>Leicht</th>
              <th>Tief</th>
              <th>REM</th>
              <th>Tage</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((month) => (
              <tr key={month.monthKey}>
                <td>{formatMonthLabel(month.monthKey)}</td>
                <td>{formatDurationMinutes(month.averageSleepMinutes)}</td>
                <td>{formatDurationMinutes(month.totalDurationMinutes)}</td>
                <td>{formatNumber(month.sessionCount)}</td>
                <td>{formatPercentage(month.awakePercentage)}</td>
                <td>{formatPercentage(month.lightPercentage)}</td>
                <td>{formatPercentage(month.deepPercentage)}</td>
                <td>{formatPercentage(month.remPercentage)}</td>
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
      <h2>Schlafübersicht</h2>

      <table>
        <thead>
          <tr>
            <th>Zeitraum</th>
            <th>Dauer</th>
            <th>Phasen</th>
            <th>Wach</th>
            <th>Leicht</th>
            <th>Tief</th>
            <th>REM</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row.key}>
              <td>
                {formatSleepDateRange(
                  row.sleepDateStartKey,
                  row.sleepDateEndKey
                )}
              </td>
              <td>{formatDurationMinutes(row.totalDurationMinutes)}</td>
              <td>{formatNumber(row.totalStageCount)}</td>
              <td>{formatPercentage(row.awakePercentage)}</td>
              <td>{formatPercentage(row.lightPercentage)}</td>
              <td>{formatPercentage(row.deepPercentage)}</td>
              <td>{formatPercentage(row.remPercentage)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default SleepSessionsTable;