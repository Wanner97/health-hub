import {
  formatDateTimeUtc,
  formatNumber,
} from '../../utils/activityDays/formatters';

function ImportBatchList({ batches }) {
  if (!batches?.length) {
    return <p>Es wurden keine Import-Batches gefunden.</p>;
  }

  return (
    <section className="batch-list">
      {batches.map((batch, index) => (
        <article
          key={`${batch.importedAtUtc}-${batch.exportedAtUtc}-${index}`}
          className="batch-card"
        >
          <div className="batch-top">
            <div>
              <h2 className="batch-title">
                Importiert am {formatDateTimeUtc(batch.importedAtUtc)}
              </h2>
              <p className="batch-subtitle">
                Exportiert am {formatDateTimeUtc(batch.exportedAtUtc)}
              </p>
            </div>

            <span className="batch-version">
              Export v{formatNumber(batch.exportVersion)}
            </span>
          </div>

          <p className="batch-range">
            Zeitraum: {formatDateTimeUtc(batch.rangeStartUtc)} –{' '}
            {formatDateTimeUtc(batch.rangeEndUtc)}
          </p>

          <div className="batch-metrics">
            <div className="metric-item">
              <span className="metric-label">Erhalten</span>
              <span className="metric-value">
                {formatNumber(batch.receivedRecordCount)}
              </span>
            </div>

            <div className="metric-item">
              <span className="metric-label">Neu</span>
              <span className="metric-value">
                {formatNumber(batch.insertedRecordCount)}
              </span>
            </div>

            <div className="metric-item">
              <span className="metric-label">Aktualisiert</span>
              <span className="metric-value">
                {formatNumber(batch.updatedRecordCount)}
              </span>
            </div>

            <div className="metric-item">
              <span className="metric-label">Unverändert</span>
              <span className="metric-value">
                {formatNumber(batch.unchangedRecordCount)}
              </span>
            </div>
          </div>
        </article>
      ))}
    </section>
  );
}

export default ImportBatchList;