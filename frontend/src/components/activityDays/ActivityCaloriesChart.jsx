import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthDetailsLabel,
} from '../../utils/date/dateFormatters';
import { formatCaloriesKcal } from '../../utils/number/numberFormatters';
import {
  buildGuideValues,
  getChartMax,
} from '../../utils/charts/scaleUtils';
import { shouldShowChartLabel } from '../../utils/charts/labelUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';

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

  if (period === PERIODS.TWELVE_MONTHS) {
    return `Ø ${formatCaloriesKcal(item.averageCaloriesBurnedKcal)} / Tag`;
  }

  return formatCaloriesKcal(item.totalCaloriesBurnedKcal);
}

function ActivityCaloriesChart({ period, data }) {
  const {
    displayedItem,
    handleItemMouseEnter,
    handleItemMouseLeave,
    handleItemClick,
  } = useSelectableChartItem({
    resetSelectors: ['.bar-column-wrapper'],
  });

  if (!data?.length) {
    return (
      <section className="chart-section">
        <h2>Kalorienverbrauch</h2>
        <p>Keine Daten vorhanden.</p>
      </section>
    );
  }

  const valueKey =
    period === PERIODS.TWELVE_MONTHS
      ? 'averageCaloriesBurnedKcal'
      : 'totalCaloriesBurnedKcal';

  const maxValue = Math.max(...data.map((item) => item[valueKey] ?? 0), 1);
  const chartMax = getChartMax(maxValue, 100);
  const guideValues = buildGuideValues(chartMax, 100);

  function getBarHeight(value) {
    return `${(value / chartMax) * 100}%`;
  }

  return (
    <section className="chart-section">
      <div className="chart-header">
        <h2>Kalorienverbrauch</h2>
      </div>

      <div className="chart-body">
        <div className="chart-canvas">
          {guideValues.map((value) => (
            <div
              key={value}
              className="chart-guide"
              style={{ bottom: `${(value / chartMax) * 100}%` }}
            >
              <span className="chart-guide-label">{Math.round(value)}</span>
            </div>
          ))}

          <div className="bar-chart-columns">
            {data.map((item) => {
              const value = item[valueKey] ?? 0;
              const isActive = displayedItem?.key === item.key;

              return (
                <div
                  key={item.key}
                  className={`bar-column-wrapper ${isActive ? 'is-active' : ''}`}
                  onMouseEnter={() => handleItemMouseEnter(item)}
                  onMouseLeave={handleItemMouseLeave}
                  onClick={() => handleItemClick(item)}
                >
                  <div
                    className="bar-fill"
                    style={{ height: getBarHeight(value) }}
                  />
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
          className={`chart-details ${displayedItem ? '' : 'chart-details--empty'}`}
        >
          <div className="chart-details-label">
            {getDetailsLabel(displayedItem, period)}
          </div>
          <div className="chart-details-value">
            {getDetailsValue(displayedItem, period)}
          </div>
        </div>
      </div>
    </section>
  );
}

export default ActivityCaloriesChart;