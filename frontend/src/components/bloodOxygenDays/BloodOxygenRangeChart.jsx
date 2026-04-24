import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthDetailsLabel,
} from '../../utils/date/dateFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';
import { getNiceStep } from '../../utils/charts/scaleUtils';
import { shouldShowChartLabel } from '../../utils/charts/labelUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';

function formatPercent(value) {
  return `${formatNumber(Math.round(value ?? 0))}%`;
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

function getDetailsValue(item) {
  if (!item) {
    return '';
  }

  return `${formatPercent(item.minPercent)} – ${formatPercent(item.maxPercent)}`;
}

function getDetailsMeta(item, period) {
  if (!item) {
    return '';
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return `Ø ${formatPercent(item.averagePercent)} · ${formatNumber(item.measurementCount)} Messungen · ${formatNumber(item.dayCount)} Tage`;
  }

  return `Ø ${formatPercent(item.avgPercent)} · ${formatNumber(item.measurementCount)} Messungen`;
}

function buildRangeGuideValues(minValue, maxValue) {
  const padding = 1;
  const paddedMin = Math.max(0, minValue - padding);
  const paddedMax = Math.min(100, maxValue + padding);
  const step = getNiceStep(Math.max(paddedMax - paddedMin, 1), 5);

  const chartMin = Math.floor(paddedMin / step) * step;
  const chartMax = Math.ceil(paddedMax / step) * step;
  const values = [];

  for (let value = chartMin; value <= chartMax; value += step) {
    values.push(value);
  }

  return {
    chartMin,
    chartMax,
    values,
  };
}

function getRelativePosition(value, chartMin, chartMax) {
  if (chartMax <= chartMin) {
    return 0;
  }

  return ((value - chartMin) / (chartMax - chartMin)) * 100;
}

function BloodOxygenRangeChart({ period, data }) {
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
        <h2>Blutsauerstoffverlauf</h2>
        <p>Keine Daten vorhanden.</p>
      </section>
    );
  }

  const minValue = Math.min(...data.map((item) => item.minPercent ?? 0));
  const maxValue = Math.max(...data.map((item) => item.maxPercent ?? 0), 1);

  const { chartMin, chartMax, values: guideValues } = buildRangeGuideValues(
    minValue,
    maxValue
  );

  return (
    <section className="chart-section">
      <div className="chart-header">
        <h2>Blutsauerstoffverlauf</h2>
      </div>

      <div className="chart-body">
        <div className="chart-canvas">
          {guideValues.map((value) => (
            <div
              key={value}
              className="chart-guide"
              style={{
                bottom: `${getRelativePosition(value, chartMin, chartMax)}%`,
              }}
            >
              <span className="chart-guide-label">{formatNumber(value)}</span>
            </div>
          ))}

          <div className="bar-chart-columns">
            {data.map((item) => {
              const isActive = displayedItem?.key === item.key;
              const bottom = getRelativePosition(item.minPercent, chartMin, chartMax);
              const top = getRelativePosition(item.maxPercent, chartMin, chartMax);
              const height = Math.max(top - bottom, 0);

              return (
                <div
                  key={item.key}
                  className={`bar-column-wrapper ${isActive ? 'is-active' : ''}`}
                  onMouseEnter={() => handleItemMouseEnter(item)}
                  onMouseLeave={handleItemMouseLeave}
                  onClick={() => handleItemClick(item)}
                >
                  <div className="blood-oxygen-range-bar-track">
                    <div
                      className="blood-oxygen-range-bar-fill"
                      style={{
                        bottom: `${bottom}%`,
                        height: `${height}%`,
                      }}
                    />
                  </div>
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

        <div className={`chart-details ${displayedItem ? '' : 'chart-details--empty'}`}>
          <div className="chart-details-label">
            {getDetailsLabel(displayedItem, period)}
          </div>

          <div className="chart-details-value">
            {getDetailsValue(displayedItem)}
          </div>

          <div
            className={`blood-oxygen-chart-details-meta ${
              displayedItem ? '' : 'blood-oxygen-chart-details-meta--empty'
            }`}
          >
            {getDetailsMeta(displayedItem, period)}
          </div>
        </div>
      </div>
    </section>
  );
}

export default BloodOxygenRangeChart;