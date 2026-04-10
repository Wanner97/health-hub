import { useEffect, useMemo, useState } from 'react';
import './App.css';
import { getActivityDays } from './api/activityDaysApi';
import ActivityDaysTable from './components/ActivityDaysTable';
import SummaryCard from './components/SummaryCard';
import { PERIODS, formatDateForInput, getPeriodLabel, getRangeFromPeriod } from './utils/activityDays/periodUtils';
import { formatKilometersFromMeters, formatNumber } from './utils/activityDays/formatters';
import {
  calculateAverageDistanceMeters,
  calculateAverageSteps,
  calculateTotalSteps,
} from './utils/activityDays/calculations';
import {
  buildMonthlyAverageRows,
  fillMissingDays,
  sortActivityDaysByDate,
  sortActivityDaysByDateDesc,
} from './utils/activityDays/transformers';

function App() {
  const [period, setPeriod] = useState(PERIODS.SEVEN_DAYS);
  const [endDate, setEndDate] = useState(formatDateForInput(new Date()));
  const [activityDays, setActivityDays] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  async function loadActivityDays(nextPeriod, nextEndDate) {
    try {
      setIsLoading(true);
      setErrorMessage('');

      const range = getRangeFromPeriod(nextPeriod, nextEndDate);
      const data = await getActivityDays(range);

      const sorted = sortActivityDaysByDate(data);
      const completed = fillMissingDays(sorted, range.from, range.to);

      setActivityDays(completed);
    } catch (error) {
      console.error('Fehler beim Laden der Activity Days:', error);
      setErrorMessage('Die Activity-Daten konnten nicht geladen werden.');
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadActivityDays(period, endDate);
  }, [period, endDate]);

  const totalSteps = useMemo(() => {
    return calculateTotalSteps(activityDays);
  }, [activityDays]);

  const averageSteps = useMemo(() => {
    return calculateAverageSteps(activityDays);
  }, [activityDays]);

  const averageDistance = useMemo(() => {
    return calculateAverageDistanceMeters(activityDays);
  }, [activityDays]);

  const displayRows = useMemo(() => {
    if (period === PERIODS.TWELVE_MONTHS) {
      return buildMonthlyAverageRows(activityDays);
    }

    return sortActivityDaysByDateDesc(activityDays);
  }, [period, activityDays]);

  return (
    <main className="app">
      <div className="container">
        <h1>Health Hub</h1>
        <p className="subtitle">Schritte mit umschaltbaren Statistikzeiträumen</p>

        <section className="filter-bar">
          <div className="period-tabs">
            <button
              type="button"
              className={period === PERIODS.SEVEN_DAYS ? 'active' : ''}
              onClick={() => setPeriod(PERIODS.SEVEN_DAYS)}
            >
              7 Tage
            </button>

            <button
              type="button"
              className={period === PERIODS.THIRTY_ONE_DAYS ? 'active' : ''}
              onClick={() => setPeriod(PERIODS.THIRTY_ONE_DAYS)}
            >
              31 Tage
            </button>

            <button
              type="button"
              className={period === PERIODS.TWELVE_MONTHS ? 'active' : ''}
              onClick={() => setPeriod(PERIODS.TWELVE_MONTHS)}
            >
              12 Monate
            </button>
          </div>

          <div className="filter-group">
            <label htmlFor="endDate">Bis</label>
            <input
              id="endDate"
              type="date"
              value={endDate}
              onChange={(event) => setEndDate(event.target.value)}
            />
          </div>
        </section>

        {isLoading && <p>Lade Daten...</p>}
        {errorMessage && <p className="error">{errorMessage}</p>}

        {!isLoading && !errorMessage && (
          <>
            <section className="summary-grid">
              <SummaryCard title="Zeitraum" value={getPeriodLabel(period)} />
              <SummaryCard title="Tage im Datensatz" value={formatNumber(activityDays.length)} />
              <SummaryCard title="Ø Schritte / Tag" value={formatNumber(averageSteps)} />
              <SummaryCard title="Ø km / Tag" value={formatKilometersFromMeters(averageDistance)} />
              <SummaryCard title="Schritte insgesamt" value={formatNumber(totalSteps)} />
            </section>

            <ActivityDaysTable rows={displayRows} period={period} />
          </>
        )}
      </div>
    </main>
  );
}

export default App;