import { APP_SECTIONS } from '../../constants/appSections';
import { useHomepageDashboard } from '../../hooks/useHomepageDashboard';
import { FRONTEND_VERSION, SUITE_VERSION } from '../../generated/version.generated';
import {
  buildBloodOxygenSubtitle,
  buildBloodOxygenTitle,
  buildHeartRateSubtitle,
  buildHeartRateTitle,
  buildImportSubtitle,
  buildSleepSubtitle,
  buildSleepTitle,
  buildActivitySubtitle,
  buildActivityTitle,
  buildWeightSubtitle,
  buildWeightTitle,
  buildNutritionSubtitle,
  buildNutritionTitle,
} from '../../utils/home/dashboardCardFormatters';

function HomeDashboard({ onSelectSection }) {
  const { dashboard, isLoading, errorMessage } = useHomepageDashboard();

  const latestImport = dashboard?.latestImport ?? null;
  const latestActivityDay = dashboard?.latestActivityDay ?? null;
  const latestSleepSession = dashboard?.latestSleepSession ?? null;
  const latestHeartRateDay = dashboard?.latestHeartRateDay ?? null;
  const latestBloodOxygenDay = dashboard?.latestBloodOxygenDay ?? null;
  const latestWeightMeasurement = dashboard?.latestWeightMeasurement ?? null;
  const latestNutritionDay = dashboard?.latestNutritionDay ?? null;

  return (
    <section className="home-section">
      <h1>Health Hub</h1>
      <p className="subtitle">Frontend Version: {FRONTEND_VERSION}</p>
      <p className="subtitle">Health-Hub Version: {SUITE_VERSION}</p>

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
          className="home-card home-card--activity"
          onClick={() => onSelectSection(APP_SECTIONS.ACTIVITY_DAYS)}
        >
          <h2>
            {isLoading
              ? 'Aktivitätsdaten werden geladen...'
              : buildActivityTitle(latestActivityDay)}
          </h2>
          <p>
            {isLoading
              ? 'Bitte warten...'
              : buildActivitySubtitle(latestActivityDay)}
          </p>
        </button>

        <button
          type="button"
          className="home-card home-card--nutrition"
          onClick={() => onSelectSection(APP_SECTIONS.NUTRITION_DAYS)}
        >
          <h2>
            {isLoading
              ? 'Ernährungsdaten werden geladen...'
              : buildNutritionTitle(latestNutritionDay)}
          </h2>
          <p>
            {isLoading
              ? 'Bitte warten...'
              : buildNutritionSubtitle(latestNutritionDay)}
          </p>
        </button>

        <button
          type="button"
          className="home-card home-card--weight"
          onClick={() => onSelectSection(APP_SECTIONS.WEIGHT_MEASUREMENTS)}
        >
          <h2>
            {isLoading
              ? 'Gewichtsdaten werden geladen...'
              : buildWeightTitle(latestWeightMeasurement)}
          </h2>
          <p>
            {isLoading
              ? 'Bitte warten...'
              : buildWeightSubtitle(latestWeightMeasurement)}
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

        <button
          type="button"
          className="home-card home-card--heart-rate"
          onClick={() => onSelectSection(APP_SECTIONS.HEART_RATE_DAYS)}
        >
          <h2>
            {isLoading
              ? 'Herzfrequenzdaten werden geladen...'
              : buildHeartRateTitle(latestHeartRateDay)}
          </h2>
          <p>
            {isLoading
              ? 'Bitte warten...'
              : buildHeartRateSubtitle(latestHeartRateDay)}
          </p>
        </button>

        <button
          type="button"
          className="home-card home-card--blood-oxygen"
          onClick={() => onSelectSection(APP_SECTIONS.BLOOD_OXYGEN_DAYS)}
        >
          <h2>
            {isLoading
              ? 'Blutsauerstoffdaten werden geladen...'
              : buildBloodOxygenTitle(latestBloodOxygenDay)}
          </h2>
          <p>
            {isLoading
              ? 'Bitte warten...'
              : buildBloodOxygenSubtitle(latestBloodOxygenDay)}
          </p>
        </button>
      </div>
    </section>
  );
}

export default HomeDashboard;