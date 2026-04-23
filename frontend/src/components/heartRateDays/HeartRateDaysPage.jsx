import PeriodSelector from '../PeriodSelector';
import ViewModeToggle from '../ViewModeToggle';
import HeartRateDaysTable from './HeartRateDaysTable';
import HeartRateRangeChart from './HeartRateRangeChart';
import HeartRateStatsSummary from './HeartRateStatsSummary';
import { VIEW_MODES } from '../../constants/viewModes';
import { useHeartRateDaysDashboard } from '../../hooks/useHeartRateDaysDashboard';
import { formatNumber } from '../../utils/number/numberFormatters';
import { formatRangeLabel } from '../../utils/periods/periodFormatters';
import {
  calculateAverageBpm,
  calculateMaxBpm,
  calculateMinBpm,
  calculateTotalMeasurements,
} from '../../utils/heartRateDays/calculations';

function HeartRateDaysPage({ onBack }) {
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
    heartRateDays,
    displayRows,
    chartData,
    dayCount,
    includeHourlyRecords,
  } = useHeartRateDaysDashboard();

  const averageBpm = calculateAverageBpm(heartRateDays);
  const minBpm = calculateMinBpm(heartRateDays);
  const maxBpm = calculateMaxBpm(heartRateDays);
  const totalMeasurements = calculateTotalMeasurements(heartRateDays);

  return (
    <section className="heart-rate-page">
      <div className="page-header">
        <button type="button" className="back-button" onClick={onBack}>
          ← Zurück zum Dashboard
        </button>
      </div>

      <h1>Health Hub</h1>
      <p className="subtitle">Puls</p>

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

      {isLoading && <p>Daten werden geladen...</p>}
      {errorMessage && <p className="error">{errorMessage}</p>}

      {!isLoading && !errorMessage && (
        <>
          <HeartRateStatsSummary
            rangeLabel={formatRangeLabel(period, selectedRange.from, selectedRange.to)}
            dayCount={formatNumber(dayCount)}
            averageBpm={formatNumber(averageBpm)}
            minBpm={formatNumber(minBpm)}
            maxBpm={formatNumber(maxBpm)}
            totalMeasurements={formatNumber(totalMeasurements)}
          />

          {viewMode === VIEW_MODES.STATS && (
            <HeartRateRangeChart period={period} data={chartData} />
          )}

          {viewMode === VIEW_MODES.TABLE && (
            <HeartRateDaysTable rows={displayRows} period={period} />
          )}
        </>
      )}
    </section>
  );
}

export default HeartRateDaysPage;