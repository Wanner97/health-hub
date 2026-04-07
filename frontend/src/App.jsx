import { useEffect, useMemo, useState } from 'react';
import './App.css';
import { getActivityDays } from './api/activityDaysApi';
import ActivityDaysTable from './components/ActivityDaysTable';
import DateRangeFilter from './components/DateRangeFilter';
import SummaryCard from './components/SummaryCard';
import {
  calculateAverageSteps,
  calculateLastNDaysAverage,
  calculateTotalDistanceMeters,
  calculateTotalSteps,
  formatDate,
  formatMeters,
  formatNumber,
  getDateRangeLabel,
  getHighestStepDay,
  getLowestStepDay,
  sortActivityDaysByDate,
} from './utils/activityDayUtils';

function App() {
  const [activityDays, setActivityDays] = useState([]);
  const [filters, setFilters] = useState({
    from: '',
    to: '',
  });
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  async function loadActivityDays(nextFilters = filters) {
    try {
      setIsLoading(true);
      setErrorMessage('');

      const data = await getActivityDays(nextFilters);
      setActivityDays(sortActivityDaysByDate(data));
    } catch (error) {
      console.error('Fehler beim Laden der Activity Days:', error);
      setErrorMessage('Die Activity-Daten konnten nicht geladen werden.');
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadActivityDays({ from: '', to: '' });
  }, []);

  function handleFilterChange(event) {
    const { name, value } = event.target;

    setFilters((current) => ({
      ...current,
      [name]: value,
    }));
  }

  async function handleApplyFilters(event) {
    event.preventDefault();

    if (filters.from && filters.to && filters.from > filters.to) {
      setErrorMessage('Das Von-Datum darf nicht nach dem Bis-Datum liegen.');
      return;
    }

    await loadActivityDays(filters);
  }

  async function handleResetFilters() {
    const resetFilters = { from: '', to: '' };
    setFilters(resetFilters);
    await loadActivityDays(resetFilters);
  }

  const totalSteps = useMemo(() => {
    return calculateTotalSteps(activityDays);
  }, [activityDays]);

  const averageSteps = useMemo(() => {
    return calculateAverageSteps(activityDays);
  }, [activityDays]);

  const last7DaysAverage = useMemo(() => {
    return calculateLastNDaysAverage(activityDays, 7);
  }, [activityDays]);

  const totalDistanceMeters = useMemo(() => {
    return calculateTotalDistanceMeters(activityDays);
  }, [activityDays]);

  const highestDay = useMemo(() => {
    return getHighestStepDay(activityDays);
  }, [activityDays]);

  const lowestDay = useMemo(() => {
    return getLowestStepDay(activityDays);
  }, [activityDays]);

  const dateRangeLabel = useMemo(() => {
    return getDateRangeLabel(activityDays);
  }, [activityDays]);

  return (
    <main className="app">
      <div className="container">
        <h1>Health Hub</h1>
        <p className="subtitle">Aktivitätsübersicht mit ersten statistischen Kennzahlen</p>

        <DateRangeFilter
          filters={filters}
          onChange={handleFilterChange}
          onApply={handleApplyFilters}
          onReset={handleResetFilters}
          disabled={isLoading}
        />

        {isLoading && <p>Lade Daten...</p>}

        {errorMessage && <p className="error">{errorMessage}</p>}

        {!isLoading && !errorMessage && activityDays.length > 0 && (
          <>
            <section className="summary-grid">
              <SummaryCard title="Zeitraum" value={dateRangeLabel} />
              <SummaryCard title="Tage" value={formatNumber(activityDays.length)} />
              <SummaryCard title="Total Steps" value={formatNumber(totalSteps)} />
              <SummaryCard title="Ø Schritte / Tag" value={formatNumber(averageSteps)} />
              <SummaryCard title="Ø letzte 7 Tage" value={formatNumber(last7DaysAverage)} />
              <SummaryCard title="Distanz total (m)" value={formatMeters(totalDistanceMeters)} />
              <SummaryCard
                title="Bester Tag"
                value={
                  highestDay
                    ? `${formatDate(highestDay.date)} – ${formatNumber(highestDay.steps)}`
                    : '-'
                }
              />
              <SummaryCard
                title="Schwächster Tag"
                value={
                  lowestDay
                    ? `${formatDate(lowestDay.date)} – ${formatNumber(lowestDay.steps)}`
                    : '-'
                }
              />
            </section>

            <ActivityDaysTable activityDays={activityDays} />
          </>
        )}

        {!isLoading && !errorMessage && activityDays.length === 0 && (
          <p>Für den gewählten Zeitraum wurden keine Daten gefunden.</p>
        )}
      </div>
    </main>
  );
}

export default App;