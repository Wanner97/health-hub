import {
  formatDate,
  formatDateTimeUtc,
  formatMeters,
  formatNumber,
} from '../utils/activityDayUtils';

function ActivityDaysTable({ activityDays }) {
  return (
    <section className="table-section">
      <h2>Activity Days</h2>

      <table>
        <thead>
          <tr>
            <th>Datum</th>
            <th>Schritte</th>
            <th>Distanz (m)</th>
            <th>Start (UTC)</th>
            <th>Ende (UTC)</th>
          </tr>
        </thead>
        <tbody>
          {activityDays.map((day) => (
            <tr key={day.date}>
              <td>{formatDate(day.date)}</td>
              <td>{formatNumber(day.steps)}</td>
              <td>{formatMeters(day.distanceMeters)}</td>
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