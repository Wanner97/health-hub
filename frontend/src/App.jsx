import { useEffect, useMemo, useState } from 'react';
import { getActivityDays } from './api/activityDaysApi';
import { getChartData } from './utils/activityDays/chartData';
import ActivityBarChart from './components/activityDays/ActivityBarChart';
import ActivityDaysTable from './components/activityDays/ActivityDaysTable';
import ActivityStatsSummary from './components/activityDays/ActivityStatsSummary';
import PeriodSelector from './components/activityDays/PeriodSelector';
import ViewModeToggle from './components/activityDays/ViewModeToggle';
import { PERIODS, formatDateForInput, getRangeFromPeriod } from './utils/activityDays/periodUtils';
import {
  formatKilometersFromMeters,
  formatNumber,
  formatRangeLabel,
} from './utils/activityDays/formatters';
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
  const [viewMode, setViewMode] = useState('stats');
  const [activityDays, setActivityDays] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');
  const selectedRange = useMemo(() => {
    return getRangeFromPeriod(period, endDate);
  }, [period, endDate]);

  async function loadActivityDays(range) {
    try {
      setIsLoading(true);
      setErrorMessage('');

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
    loadActivityDays(selectedRange);
  }, [selectedRange]);

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

  const chartData = useMemo(() => {
    return getChartData(period, activityDays);
  }, [period, activityDays]);

  return (
    <main className="app">
      <div className="container">
        <h1>Health Hub</h1>
        <p className="subtitle">Schritte mit umschaltbaren Statistikzeiträumen</p>

        <PeriodSelector
          period={period}
          endDate={endDate}
          onPeriodChange={setPeriod}
          onEndDateChange={setEndDate}
        />

        <ViewModeToggle
          viewMode={viewMode}
          onViewModeChange={setViewMode}
        />

        {isLoading && <p>Lade Daten...</p>}
        {errorMessage && <p className="error">{errorMessage}</p>}

        {!isLoading && !errorMessage && (
          <>
            <ActivityStatsSummary
              rangeLabel={formatRangeLabel(period, selectedRange.from, selectedRange.to)}
              dayCount={formatNumber(activityDays.length)}
              averageSteps={formatNumber(averageSteps)}
              averageDistance={formatKilometersFromMeters(averageDistance)}
              totalSteps={formatNumber(totalSteps)}
            />

            {viewMode === 'stats' && (
              <ActivityBarChart period={period} data={chartData} />
            )}

            {viewMode === 'table' && (
              <ActivityDaysTable rows={displayRows} period={period} />
            )}
          </>
        )}
      </div>
    </main>
  );
}

export default App;