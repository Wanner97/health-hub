import { formatNumber } from '../../utils/number/numberFormatters';
import { getNiceStep } from '../../utils/charts/scaleUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';
import { buildHourlyHeartRateChartData } from '../../utils/heartRateDays/hourlyChartData';

function buildRangeGuideValues(minValue, maxValue) {
  const padding = 5;
  const paddedMin = Math.max(0, minValue - padding);
  const paddedMax = maxValue + padding;
  const step = getNiceStep(Math.max(paddedMax - paddedMin, 1), 20);

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
  if (value == null || chartMax <= chartMin) {
    return 0;
  }

  return ((value - chartMin) / (chartMax - chartMin)) * 100;
}

function shouldShowHourLabel(hour) {
  return hour % 3 === 0;
}

function getHourlyDetailsLabel(item) {
  if (!item || !item.hasData) {
    return '';
  }

  return item.fullLabel;
}

function getHourlyDetailsValue(item) {
  if (!item || !item.hasData) {
    return '';
  }

  return `${formatNumber(item.minBpm)}–${formatNumber(item.maxBpm)} BPM`;
}

function getHourlyDetailsMeta(item) {
  if (!item || !item.hasData) {
    return '';
  }

  return `Ø ${formatNumber(item.avgBpm)} BPM · ${formatNumber(item.measurementCount)} Messungen`;
}

function HeartRateHourlyChart({ dateLabel, hourlyRecords }) {
  const data = buildHourlyHeartRateChartData(hourlyRecords);
  const dataWithValues = data.filter((item) => item.hasData);

  const {
    displayedItem,
    handleItemMouseEnter,
    handleItemMouseLeave,
    handleItemClick,
  } = useSelectableChartItem({
    resetSelectors: ['.heart-rate-hour-slot'],
  });

  if (!dataWithValues.length) {
    return (
      <section className="heart-rate-hourly-details">
        <h3 className="heart-rate-hourly-title">Stundenwerte für {dateLabel}</h3>
        <p className="heart-rate-hourly-empty">
          Für diesen Tag sind keine stündlichen Pulsdaten vorhanden.
        </p>
      </section>
    );
  }

  const minValue = Math.min(...dataWithValues.map((item) => item.minBpm ?? 0));
  const maxValue = Math.max(...dataWithValues.map((item) => item.maxBpm ?? 0), 1);

  const { chartMin, chartMax, values: guideValues } = buildRangeGuideValues(
    minValue,
    maxValue
  );

  return (
    <section className="heart-rate-hourly-details">
      <h3 className="heart-rate-hourly-title">Stundenwerte für {dateLabel}</h3>

      <div className="heart-rate-hourly-chart">
        <div className="heart-rate-hourly-canvas">
          {guideValues.map((value) => (
            <div
              key={value}
              className="heart-rate-hourly-guide"
              style={{
                bottom: `${getRelativePosition(value, chartMin, chartMax)}%`,
              }}
            >
              <span className="heart-rate-hourly-guide-label">
                {formatNumber(value)}
              </span>
            </div>
          ))}

          <div className="heart-rate-hourly-columns">
            {data.map((item) => {
              const bottom = getRelativePosition(item.minBpm, chartMin, chartMax);
              const top = getRelativePosition(item.maxBpm, chartMin, chartMax);
              const height = Math.max(top - bottom, 0);
              const isActive = displayedItem?.key === item.key;

              return (
                <div
                  key={item.key}
                  className={`heart-rate-hour-slot ${
                    item.hasData ? 'heart-rate-hour-slot--has-data' : 'heart-rate-hour-slot--empty'
                  } ${isActive ? 'is-active' : ''}`}
                  title={
                    item.hasData
                      ? `${item.fullLabel} · ${formatNumber(item.minBpm)}–${formatNumber(item.maxBpm)} BPM`
                      : `${item.fullLabel} · keine Daten`
                  }
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
                  <div className="heart-rate-hour-slot-track">
                    {item.hasData && (
                      <div
                        className="heart-rate-hour-slot-fill"
                        style={{
                          bottom: `${bottom}%`,
                          height: `${height}%`,
                        }}
                      />
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        <div className="heart-rate-hourly-labels">
          {data.map((item) => (
            <div key={`${item.key}-label`} className="heart-rate-hourly-label">
              {shouldShowHourLabel(item.hour) ? item.label : ''}
            </div>
          ))}
        </div>

        <div
          className={`chart-details heart-rate-hourly-record-details ${
            displayedItem ? '' : 'chart-details--empty'
          }`}
        >
          <div className="chart-details-label">
            {getHourlyDetailsLabel(displayedItem)}
          </div>

          <div className="chart-details-value">
            {getHourlyDetailsValue(displayedItem)}
          </div>

          <div
            className={`heart-rate-hourly-record-meta ${
              displayedItem ? '' : 'heart-rate-hourly-record-meta--empty'
            }`}
          >
            {getHourlyDetailsMeta(displayedItem)}
          </div>
        </div>
      </div>
    </section>
  );
}

export default HeartRateHourlyChart;