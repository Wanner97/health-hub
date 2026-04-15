import { APP_SECTIONS } from '../../constants/appSections';
import { useHomepageDashboard } from '../../hooks/useHomepageDashboard';
import {
  formatDate,
  formatDateUtcDateOnly,
} from '../../utils/date/dateFormatters';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';

function buildImportSubtitle(latestImport) {
  if (!latestImport) {
    return 'Noch keine Importdaten verfügbar.';
  }

  return `${formatDateUtcDateOnly(latestImport.importedAtUtc)} · Erhalten ${formatNumber(latestImport.receivedRecordCount)} · Neu ${formatNumber(latestImport.insertedRecordCount)} · Aktualisiert ${formatNumber(latestImport.updatedRecordCount)} · Unverändert ${formatNumber(latestImport.unchangedRecordCount)}`;
}

function buildStepsTitle(latestActivityDay) {
  if (!latestActivityDay) {
    return 'Keine Schrittedaten';
  }

  return `${formatNumber(latestActivityDay.steps)} Schritte`;
}

function buildStepsSubtitle(latestActivityDay) {
  if (!latestActivityDay) {
    return 'Kein aktueller Schrittedatensatz vorhanden.';
  }

  return `am ${formatDate(latestActivityDay.date)}`;
}

function buildSleepTitle(latestSleepSession) {
  if (!latestSleepSession) {
    return 'Kein Schlafdatensatz';
  }

  return `${formatDurationMinutes(latestSleepSession.durationMinutes)} Schlaf`;
}

function buildSleepSubtitle(latestSleepSession) {
  if (!latestSleepSession) {
    return 'Kein aktueller Schlafdatensatz vorhanden.';
  }

  const startDate = formatDateUtcDateOnly(latestSleepSession.startTimeUtc);
  const endDate = formatDateUtcDateOnly(latestSleepSession.endTimeUtc);

  return `vom ${startDate} auf den ${endDate}`;
}

function HomeSectionSelector({ onSelectSection }) {
  const { dashboard, isLoading, errorMessage } = useHomepageDashboard();

  const latestImport = dashboard?.latestImport ?? null;
  const latestActivityDay = dashboard?.latestActivityDay ?? null;
  const latestSleepSession = dashboard?.latestSleepSession ?? null;

  return (
    <section className="home-section">
      <h1>Health Hub</h1>
      <p className="subtitle">Übersicht über die aktuellsten Daten im System.</p>

      {errorMessage && <p className="error">{errorMessage}</p>}

      <div className="home-grid">
        <button
          type="button"
          className="home-card"
          onClick={() => onSelectSection(APP_SECTIONS.IMPORT_BATCHES)}
        >
          <h2>Letzter Import:</h2>
          <p>
            {isLoading
              ? 'Importdaten werden geladen...'
              : buildImportSubtitle(latestImport)}
          </p>
        </button>

        <button
          type="button"
          className="home-card home-card--steps"
          onClick={() => onSelectSection(APP_SECTIONS.ACTIVITY_DAYS)}
        >
          <h2>
            {isLoading
              ? 'Schrittedaten werden geladen...'
              : buildStepsTitle(latestActivityDay)}
          </h2>
          <p>
            {isLoading
              ? 'Bitte warten...'
              : buildStepsSubtitle(latestActivityDay)}
          </p>
        </button>

        <button
          type="button"
          className="home-card home-card--sleep"
          onClick={() => onSelectSection(APP_SECTIONS.SLEEP_SESSIONS)}
        >
          <h2>
            {isLoading
              ? 'Schlafdaten werden geladen...'
              : buildSleepTitle(latestSleepSession)}
          </h2>
          <p>
            {isLoading
              ? 'Bitte warten...'
              : buildSleepSubtitle(latestSleepSession)}
          </p>
        </button>
      </div>
    </section>
  );
}

export default HomeSectionSelector;