import { PERIODS } from '../../constants/periods';
import {
  buildRangeDetailsMeta,
  getRangeDetailsLabel,
} from '../../utils/charts/rangeDetailFormatters';
import { formatNumber, formatPercent } from '../../utils/number/numberFormatters';
import {
  buildRangeScale,
  getRangePosition,
} from '../../utils/charts/rangeScaleUtils';
import { shouldShowChartLabel } from '../../utils/charts/labelUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';

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

  const averageValue =
    period === PERIODS.TWELVE_MONTHS
      ? formatPercent(item.averagePercent)
      : formatPercent(item.avgPercent);

  return buildRangeDetailsMeta({
    period,
    averageValue,
    measurementCount: formatNumber(item.measurementCount),
    dayCount: formatNumber(item.dayCount),
  });
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

  const { chartMin, chartMax, values: guideValues } = buildRangeScale(
    minValue,
    maxValue,
    {
      padding: 1,
      minLimit: 0,
      maxLimit: 100,
      fallbackStep: 5,
    }
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
                bottom: `${getRangePosition(value, chartMin, chartMax)}%`,
              }}
            >
              <span className="chart-guide-label">{formatNumber(value)}</span>
            </div>
          ))}

          <div className="bar-chart-columns">
            {data.map((item) => {
              const isActive = displayedItem?.key === item.key;
              const bottom = getRangePosition(item.minPercent, chartMin, chartMax);
              const top = getRangePosition(item.maxPercent, chartMin, chartMax);
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
            {getRangeDetailsLabel(displayedItem, period)}
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