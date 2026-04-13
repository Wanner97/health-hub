import { APP_SECTIONS } from '../../constants/appSections';

function HomeSectionSelector({ onSelectSection }) {
  return (
    <section className="home-section">
      <h1>Health Hub</h1>
      <p className="subtitle">
        Some Text.
      </p>

      <div className="home-grid">
        <button
          type="button"
          className="home-card"
          onClick={() => onSelectSection(APP_SECTIONS.ACTIVITY_DAYS)}
        >
          <h2>Schritte & Aktivitätstage</h2>
          <p>
            Statistikansicht und Tabellenansicht für Schritte und Distanz im
            gewählten Zeitraum.
          </p>
        </button>

        <button
          type="button"
          className="home-card"
          onClick={() => onSelectSection(APP_SECTIONS.IMPORT_BATCHES)}
        >
          <h2>Import-Batches</h2>
          <p>
            Übersicht über Importe, Exportversionen sowie neu eingefügte,
            aktualisierte und unveränderte Datensätze.
          </p>
        </button>
      </div>
    </section>
  );
}

export default HomeSectionSelector;