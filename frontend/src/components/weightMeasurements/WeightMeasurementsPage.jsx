import PeriodSelector from '../PeriodSelector';
import ViewModeToggle from '../ViewModeToggle';
import { VIEW_MODES } from '../../constants/viewModes';
import { useWeightMeasurementsDashboard } from '../../hooks/useWeightMeasurementsDashboard';
import { formatNumber, formatWeightKg } from '../../utils/number/numberFormatters';
import { formatRangeLabel } from '../../utils/periods/periodFormatters';
import WeightMeasurementsStatsSummary from './WeightMeasurementsStatsSummary';
import WeightMeasurementsTable from './WeightMeasurementsTable';
import WeightLineChart from './WeightLineChart';

function WeightMeasurementsPage({ onBack }) {
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
    displayRows,
    chartData,
    summaryData,
    usesUnboundedFallback,
  } = useWeightMeasurementsDashboard();

  return (
    <section className="weight-page">
      <div className="page-header">
        <button type="button" className="back-button" onClick={onBack}>
          ← Zurück zum Dashboard
        </button>
      </div>

      <h1>Health Hub</h1>
      <p className="subtitle">Körpergewicht</p>

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
        <WeightMeasurementsStatsSummary
          rangeLabel={
            usesUnboundedFallback
              ? 'Unbegrenzte Abfrage'
              : formatRangeLabel(period, selectedRange.from, selectedRange.to)
          }
          measurementCount={formatNumber(summaryData.measurementCount)}
          averageWeightKg={formatWeightKg(summaryData.averageWeightKg)}
          minWeightKg={formatWeightKg(summaryData.minWeightKg)}
          maxWeightKg={formatWeightKg(summaryData.maxWeightKg)}
          usesUnboundedFallback={usesUnboundedFallback}
        />
      )}

      {!isLoading && !errorMessage && viewMode === VIEW_MODES.STATS && (
        <WeightLineChart
          period={period}
          data={chartData}
          usesUnboundedFallback={usesUnboundedFallback}
        />
      )}

      {!isLoading && !errorMessage && viewMode === VIEW_MODES.TABLE && (
        <WeightMeasurementsTable rows={displayRows} period={period} />
      )}
    </section>
  );
}

export default WeightMeasurementsPage;