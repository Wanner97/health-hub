import PeriodSelector from '../PeriodSelector';
import ViewModeToggle from '../ViewModeToggle';
import { VIEW_MODES } from '../../constants/viewModes';
import { useNutritionDaysDashboard } from '../../hooks/useNutritionDaysDashboard';
import { formatCaloriesKcal, formatNumber } from '../../utils/number/numberFormatters';
import { formatRangeLabel } from '../../utils/periods/periodFormatters';
import {
  calculateAverageEnergyKcalPerDay,
  calculateNutritionDayCount,
  calculateTotalEnergyKcal,
  calculateTotalNutritionRecords,
} from '../../utils/nutritionDays/calculations';
import NutritionDaysStatsSummary from './NutritionDaysStatsSummary';
import NutritionDaysTable from './NutritionDaysTable';
import NutritionCaloriesChart from './NutritionCaloriesChart';

function NutritionDaysPage({ onBack }) {
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
    nutritionDays,
    displayRows,
    chartData,
  } = useNutritionDaysDashboard();

  const dayCount = calculateNutritionDayCount(nutritionDays);
  const totalRecordCount = calculateTotalNutritionRecords(nutritionDays);
  const totalEnergyKcal = calculateTotalEnergyKcal(nutritionDays);
  const averageEnergyKcal = calculateAverageEnergyKcalPerDay(nutritionDays);

  return (
    <section className="nutrition-page">
      <div className="page-header">
        <button type="button" className="back-button" onClick={onBack}>
          ← Zurück zum Dashboard
        </button>
      </div>

      <h1>Health Hub</h1>
      <p className="subtitle">Ernährung</p>

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

      {!isLoading && !errorMessage && (
        <>
          <NutritionDaysStatsSummary
            rangeLabel={formatRangeLabel(period, selectedRange.from, selectedRange.to)}
            dayCount={formatNumber(dayCount)}
            averageEnergyKcal={formatCaloriesKcal(averageEnergyKcal)}
            totalRecordCount={formatNumber(totalRecordCount)}
            totalEnergyKcal={formatCaloriesKcal(totalEnergyKcal)}
          />

          {viewMode === VIEW_MODES.STATS && (
            <NutritionCaloriesChart period={period} data={chartData} />
          )}

          {viewMode === VIEW_MODES.TABLE && (
            <NutritionDaysTable rows={displayRows} />
          )}
        </>
      )}
    </section>
  );
}

export default NutritionDaysPage;