import { APP_SECTIONS } from '../../constants/appSections';
import { useHomepageDashboard } from '../../hooks/useHomepageDashboard';
import {
  formatDate,
  formatDateUtcDateOnly,
  formatNumber,
} from '../../utils/activityDays/formatters';

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

function HomeSectionSelector({ onSelectSection }) {
  const { dashboard, isLoading, errorMessage } = useHomepageDashboard();

  const latestImport = dashboard?.latestImport ?? null;
  const latestActivityDay = dashboard?.latestActivityDay ?? null;

  return (
    <section className="home-section">
      <h1>Health Hub</h1>
      <p className="subtitle">some sample text</p>

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
      </div>
    </section>
  );
}

export default HomeSectionSelector;