import { formatDateTime, formatNumber, formatDate } from '../utils/stepsUtils';

function StepsTable({ stepEntries }) {
  return (
    <section className="table-section">
      <h2>Step Entries</h2>

      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>Count</th>
            <th>Start Time</th>
            <th>End Time</th>
          </tr>
        </thead>
        <tbody>
          {stepEntries.map((entry) => (
            <tr key={entry.date}>
              <td>{formatDate(entry.date)}</td>
              <td>{formatNumber(entry.count)}</td>
              <td>{formatDateTime(entry.startTime)}</td>
              <td>{formatDateTime(entry.endTime)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default StepsTable;