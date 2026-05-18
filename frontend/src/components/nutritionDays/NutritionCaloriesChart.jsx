import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthDetailsLabel,
} from '../../utils/date/dateFormatters';
import {
  formatCaloriesKcal,
  formatGrams,
  formatNumber,
} from '../../utils/number/numberFormatters';
import {
  buildGuideValues,
  getChartMax,
} from '../../utils/charts/scaleUtils';
import { shouldShowChartLabel } from '../../utils/charts/labelUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';
import NutritionMacroBreakdown from './NutritionMacroBreakdown';
import { buildNutritionDetails } from '../../utils/nutritionDays/detailData';
import NutritionMealGroups from './NutritionMealGroups';
import NutritionNutrientTotals from './NutritionNutrientTotals';

function getEnergyValue(item, period) {
  if (!item) {
    return 0;
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return item.averageEnergyKcal ?? 0;
  }

  return item.totalEnergyKcal ?? 0;
}

function getCarbohydrateValue(item, period) {
  if (!item) {
    return 0;
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return item.averageCarbohydrateGrams ?? 0;
  }

  return item.totalCarbohydrateGrams ?? 0;
}

function getFatValue(item, period) {
  if (!item) {
    return 0;
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return item.averageFatGrams ?? 0;
  }

  return item.totalFatGrams ?? 0;
}

function getProteinValue(item, period) {
  if (!item) {
    return 0;
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return item.averageProteinGrams ?? 0;
  }

  return item.totalProteinGrams ?? 0;
}

function getDetailsLabel(item, period) {
  if (!item) {
    return '';
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return formatMonthDetailsLabel(item.fullLabel);
  }

  return formatDate(item.fullLabel);
}

function getDetailsValue(item, period) {
  if (!item) {
    return '';
  }

  const energyValue = getEnergyValue(item, period);

  if (period === PERIODS.TWELVE_MONTHS) {
    return `Ø ${formatCaloriesKcal(energyValue)} / Tag`;
  }

  return formatCaloriesKcal(energyValue);
}

function getDetailsMeta(item, period) {
  if (!item) {
    return null;
  }

  const carbohydrateValue = getCarbohydrateValue(item, period);
  const fatValue = getFatValue(item, period);
  const proteinValue = getProteinValue(item, period);

  const recordText =
    period === PERIODS.TWELVE_MONTHS
      ? `${formatNumber(item.recordCount)} Records · ${formatNumber(
          item.dayCount
        )} Tage`
      : `${formatNumber(item.recordCount)} Records`;

  return (
    <>
      <span className="nutrition-details-record-text">{recordText}</span>

      <span className="nutrition-details-macro nutrition-details-macro--carbs">
        KH {formatGrams(carbohydrateValue)}
      </span>

      <span className="nutrition-details-macro nutrition-details-macro--fat">
        Fett {formatGrams(fatValue)}
      </span>

      <span className="nutrition-details-macro nutrition-details-macro--protein">
        Protein {formatGrams(proteinValue)}
      </span>
    </>
  );
}

function getMacroSegments(item) {
  if (!item) {
    return [];
  }

  return [
    {
      key: 'carbohydrate',
      className: 'nutrition-bar-segment--carbs',
      percentage: item.carbohydratePercent ?? 0,
    },
    {
      key: 'fat',
      className: 'nutrition-bar-segment--fat',
      percentage: item.fatPercent ?? 0,
    },
    {
      key: 'protein',
      className: 'nutrition-bar-segment--protein',
      percentage: item.proteinPercent ?? 0,
    },
  ].filter((segment) => segment.percentage > 0);
}

function NutritionCaloriesChart({ period, data }) {
  const {
    displayedItem,
    handleItemMouseEnter,
    handleItemMouseLeave,
    handleItemClick,
  } = useSelectableChartItem({
    resetSelectors: ['.nutrition-bar-column-wrapper'],
  });

  const dataWithValues = data?.filter((item) => item.hasData) ?? [];

  if (!data?.length || !dataWithValues.length) {
    return (
      <section className="chart-section">
        <h2>Kalorienverlauf</h2>
        <p>Keine Ernährungsdaten vorhanden.</p>
      </section>
    );
  }

  const displayedDetails = buildNutritionDetails(displayedItem);

  const maxValue = Math.max(
    ...dataWithValues.map((item) => getEnergyValue(item, period)),
    1
  );

  const chartMax = getChartMax(maxValue, 500);
  const guideValues = buildGuideValues(chartMax, 500);

  function getBarHeight(value) {
    return `${(value / chartMax) * 100}%`;
  }

  return (
    <section className="chart-section nutrition-chart-section">
      <div className="chart-header">
        <h2>Kalorienverlauf</h2>
      </div>

      <div className="chart-body">
        <div className="chart-canvas">
          {guideValues.map((value) => (
            <div
              key={value}
              className="chart-guide"
              style={{ bottom: `${(value / chartMax) * 100}%` }}
            >
              <span className="chart-guide-label">
                {formatCaloriesKcal(value)}
              </span>
            </div>
          ))}

          <div className="bar-chart-columns">
            {data.map((item) => {
              const value = getEnergyValue(item, period);
              const isActive = displayedItem?.key === item.key;
              const macroSegments = getMacroSegments(item);

              return (
                <div
                  key={item.key}
                  className={`bar-column-wrapper nutrition-bar-column-wrapper ${
                    item.hasData ? 'nutrition-bar-column-wrapper--has-data' : ''
                  } ${isActive ? 'is-active' : ''}`}
                  onMouseEnter={() => {
                    if (item.hasData) {
                      handleItemMouseEnter(item);
                    }
                  }}
                  onMouseLeave={handleItemMouseLeave}
                  onClick={() => {
                    if (item.hasData) {
                      handleItemClick(item);
                    }
                  }}
                >
                  {item.hasData && (
                    <div
                      className="nutrition-bar-fill"
                      style={{ height: getBarHeight(value) }}
                    >
                      {isActive &&
                        macroSegments.map((segment) => (
                          <div
                            key={segment.key}
                            className={`nutrition-bar-segment ${segment.className}`}
                            style={{ height: `${segment.percentage}%` }}
                          />
                        ))}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>

        <div className="bar-chart-labels">
          {data.map((item, index) => (
            <div key={`${item.key}-label`} className="bar-label">
              {shouldShowChartLabel(period, index, data.length) ? item.label : ''}
            </div>
          ))}
        </div>

        <div
          className={`chart-details nutrition-chart-details ${
            displayedItem ? '' : 'chart-details--empty'
          }`}
        >
          <div className="chart-details-label">
            {getDetailsLabel(displayedItem, period)}
          </div>

          <div className="chart-details-value">
            {getDetailsValue(displayedItem, period)}
          </div>

          <div
            className={`nutrition-chart-details-meta ${
              displayedItem ? '' : 'nutrition-chart-details-meta--empty'
            }`}
          >
            {getDetailsMeta(displayedItem, period)}
          </div>

          {displayedItem && (
            <NutritionMacroBreakdown item={displayedItem} period={period} />
          )}

          {displayedItem && (
            <NutritionMealGroups
              mealGroups={displayedDetails.mealGroups}
              period={period}
            />
          )}

          {displayedItem && (
            <NutritionNutrientTotals
              nutrientTotals={displayedDetails.nutrientTotals}
              period={period}
            />
          )}
        </div>
      </div>
    </section>
  );
}

export default NutritionCaloriesChart;