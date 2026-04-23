import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthDetailsLabel,
} from '../../utils/date/dateFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';
import { getNiceStep } from '../../utils/charts/scaleUtils';
import { shouldShowChartLabel } from '../../utils/charts/labelUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';
import HeartRateHourlyChart from './HeartRateHourlyChart';

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

  return `${formatNumber(item.minBpm)}–${formatNumber(item.maxBpm)} BPM`;
}

function getDetailsMeta(item, period) {
  if (!item) {
    return '';
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return `Ø ${formatNumber(item.averageBpm)} BPM · ${formatNumber(item.measurementCount)} Messungen · ${formatNumber(item.dayCount)} Tage`;
  }

  return `Ø ${formatNumber(item.avgBpm)} BPM · ${formatNumber(item.measurementCount)} Messungen`;
}

function buildRangeGuideValues(minValue, maxValue) {
  const padding = 5;
  const paddedMin = Math.max(0, minValue - padding);
  const paddedMax = maxValue + padding;
  const step = getNiceStep(paddedMax - paddedMin, 20);

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

function HeartRateRangeChart({ period, data }) {
  const {
    displayedItem,
    handleItemMouseEnter,
    handleItemMouseLeave,
    handleItemClick,
    } = useSelectableChartItem({
        resetSelectors: ['.bar-column-wrapper', '.heart-rate-hourly-details'],
      });

  if (!data?.length) {
    return (
      <section className="chart-section">
        <h2>Herzfrequenzverlauf</h2>
        <p>Keine Daten vorhanden.</p>
      </section>
    );
  }

  const minValue = Math.min(...data.map((item) => item.minBpm ?? 0));
  const maxValue = Math.max(...data.map((item) => item.maxBpm ?? 0), 1);

  const { chartMin, chartMax, values: guideValues } = buildRangeGuideValues(
    minValue,
    maxValue
  );

  return (
    <section className="chart-section">
      <div className="chart-header">
        <h2>Herzfrequenzverlauf</h2>
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
              const bottom = getRelativePosition(item.minBpm, chartMin, chartMax);
              const top = getRelativePosition(item.maxBpm, chartMin, chartMax);
              const height = Math.max(top - bottom, 0);

              return (
                <div
                  key={item.key}
                  className={`bar-column-wrapper ${isActive ? 'is-active' : ''}`}
                  onMouseEnter={() => handleItemMouseEnter(item)}
                  onMouseLeave={handleItemMouseLeave}
                  onClick={() => handleItemClick(item)}
                >
                  <div className="range-bar-track">
                    <div
                      className="range-bar-fill"
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
            className={`heart-rate-chart-details-meta ${
              displayedItem ? '' : 'heart-rate-chart-details-meta--empty'
            }`}
          >
            {getDetailsMeta(displayedItem, period)}
          </div>

          {period !== PERIODS.TWELVE_MONTHS && displayedItem && (
            <HeartRateHourlyChart
              dateLabel={getDetailsLabel(displayedItem, period)}
              hourlyRecords={displayedItem.hourlyRecords ?? []}
            />
          )}
        </div>
      </div>
    </section>
  );
}

export default HeartRateRangeChart;