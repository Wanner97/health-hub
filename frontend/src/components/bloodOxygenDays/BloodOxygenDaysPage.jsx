import PeriodSelector from '../PeriodSelector';
import ViewModeToggle from '../ViewModeToggle';
import BloodOxygenDaysTable from './BloodOxygenDaysTable';
import BloodOxygenRangeChart from './BloodOxygenRangeChart';
import BloodOxygenStatsSummary from './BloodOxygenStatsSummary';
import { VIEW_MODES } from '../../constants/viewModes';
import { useBloodOxygenDaysDashboard } from '../../hooks/useBloodOxygenDaysDashboard';
import { formatNumber } from '../../utils/number/numberFormatters';
import { formatRangeLabel } from '../../utils/periods/periodFormatters';
import {
  calculateAveragePercent,
  calculateMaxPercent,
  calculateMinPercent,
  calculateTotalMeasurements,
} from '../../utils/bloodOxygenDays/calculations';

function formatPercent(value) {
  return `${formatNumber(Math.round(value ?? 0))} %`;
}

function BloodOxygenDaysPage({ onBack }) {
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
    bloodOxygenDays,
    displayRows,
    chartData,
    dayCount,
  } = useBloodOxygenDaysDashboard();

  const averagePercent = calculateAveragePercent(bloodOxygenDays);
  const minPercent = calculateMinPercent(bloodOxygenDays);
  const maxPercent = calculateMaxPercent(bloodOxygenDays);
  const totalMeasurements = calculateTotalMeasurements(bloodOxygenDays);

  return (
    <section className="blood-oxygen-page">
      <div className="page-header">
        <button type="button" className="back-button" onClick={onBack}>
          ← Zurück zum Dashboard
        </button>
      </div>

      <h1>Health Hub</h1>
      <p className="subtitle">Blutsauerstoff</p>

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
          <BloodOxygenStatsSummary
            rangeLabel={formatRangeLabel(period, selectedRange.from, selectedRange.to)}
            dayCount={formatNumber(dayCount)}
            averagePercent={formatPercent(averagePercent)}
            minPercent={formatPercent(minPercent)}
            maxPercent={formatPercent(maxPercent)}
            totalMeasurements={formatNumber(totalMeasurements)}
          />

          {viewMode === VIEW_MODES.STATS && (
            <BloodOxygenRangeChart period={period} data={chartData} />
          )}

          {viewMode === VIEW_MODES.TABLE && (
            <BloodOxygenDaysTable rows={displayRows} period={period} />
          )}
        </>
      )}
    </section>
  );
}

export default BloodOxygenDaysPage;