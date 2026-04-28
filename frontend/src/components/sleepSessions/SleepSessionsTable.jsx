import { PERIODS } from '../../constants/periods';
import { formatMonthLabel } from '../../utils/date/dateFormatters';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';
import {
  formatSleepDateRange,
  formatSleepPercentage,
} from '../../utils/sleepSessions/formatters';

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
                <td>{formatSleepPercentage(month.awakePercentage)}</td>
                <td>{formatSleepPercentage(month.lightPercentage)}</td>
                <td>{formatSleepPercentage(month.deepPercentage)}</td>
                <td>{formatSleepPercentage(month.remPercentage)}</td>
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
              <td>{formatSleepPercentage(row.awakePercentage)}</td>
              <td>{formatSleepPercentage(row.lightPercentage)}</td>
              <td>{formatSleepPercentage(row.deepPercentage)}</td>
              <td>{formatSleepPercentage(row.remPercentage)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default SleepSessionsTable;