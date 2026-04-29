import ActivityBarChart from './ActivityBarChart';
import ActivityCaloriesChart from './ActivityCaloriesChart';
import ActivityDaysTable from './ActivityDaysTable';
import ActivityStatsSummary from './ActivityStatsSummary';
import PeriodSelector from '../PeriodSelector';
import ViewModeToggle from '../ViewModeToggle';
import { VIEW_MODES } from '../../constants/viewModes';
import { useActivityDaysDashboard } from '../../hooks/useActivityDaysDashboard';
import { formatRangeLabel } from '../../utils/periods/periodFormatters';
import {
  formatKilometersFromMeters,
  formatNumber,
} from '../../utils/number/numberFormatters';

function ActivityDaysPage({ onBack }) {
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
    <section>
      <div className="page-header">
        <button type="button" className="back-button" onClick={onBack}>
          ← Zurück zum Dashboard
        </button>
      </div>

      <h1>Health Hub</h1>
      <p className="subtitle">Aktivität</p>

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
            <>
              <ActivityBarChart period={period} data={chartData} />
              <ActivityCaloriesChart period={period} data={chartData} />
            </>
          )}

          {viewMode === VIEW_MODES.TABLE && (
            <ActivityDaysTable rows={displayRows} period={period} />
          )}
        </>
      )}
    </section>
  );
}

export default ActivityDaysPage;