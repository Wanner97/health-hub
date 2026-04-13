import ActivityBarChart from './components/activityDays/ActivityBarChart';
import ActivityDaysTable from './components/activityDays/ActivityDaysTable';
import ActivityStatsSummary from './components/activityDays/ActivityStatsSummary';
import PeriodSelector from './components/activityDays/PeriodSelector';
import ViewModeToggle from './components/activityDays/ViewModeToggle';
import { VIEW_MODES } from './constants/viewModes';
import { useActivityDaysDashboard } from './hooks/useActivityDaysDashboard';
import {
  formatKilometersFromMeters,
  formatNumber,
  formatRangeLabel,
} from './utils/activityDays/formatters';

function App() {
  const {
    period,
    setPeriod,
    endDate,
    setEndDate,
    viewMode,
    setViewMode,
    isLoading,
    errorMessage,
    selectedRange,
    dayCount,
    totalSteps,
    averageSteps,
    averageDistance,
    displayRows,
    chartData,
  } = useActivityDaysDashboard();

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
              dayCount={formatNumber(dayCount)}
              averageSteps={formatNumber(averageSteps)}
              averageDistance={formatKilometersFromMeters(averageDistance)}
              totalSteps={formatNumber(totalSteps)}
            />

            {viewMode === VIEW_MODES.STATS && (
              <ActivityBarChart period={period} data={chartData} />
            )}

            {viewMode === VIEW_MODES.TABLE && (
              <ActivityDaysTable rows={displayRows} period={period} />
            )}
          </>
        )}
      </div>
    </main>
  );
}

export default App;