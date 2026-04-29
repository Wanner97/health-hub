import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthDetailsLabel,
} from '../../utils/date/dateFormatters';
import { formatNumber, formatWeightKg } from '../../utils/number/numberFormatters';
import { shouldShowChartLabel } from '../../utils/charts/labelUtils';
import { getRangePosition } from '../../utils/charts/rangeScaleUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';
import { buildWeightLineChartModel } from '../../utils/weightMeasurements/chartData';

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
    return `Ø ${formatWeightKg(item.weightKg)}`;
  }

  return formatWeightKg(item.weightKg);
}

function getDetailsMeta(item, period) {
  if (!item || period !== PERIODS.TWELVE_MONTHS) {
    return '';
  }

  return `Min ${formatWeightKg(item.minWeightKg)} · Max ${formatWeightKg(
    item.maxWeightKg
  )} · ${formatNumber(item.measurementCount)} Messungen`;
}

function WeightLineChart({ period, data, usesUnboundedFallback }) {
  const {
    displayedItem,
    handleItemMouseEnter,
    handleItemMouseLeave,
    handleItemClick,
  } = useSelectableChartItem({
    resetSelectors: ['.weight-line-point'],
  });

  const measuredItems = data?.filter((item) => item.hasMeasurement) ?? [];

  if (!data?.length || !measuredItems.length) {
    return (
      <section className="chart-section">
        <h2>Gewichtsverlauf</h2>
        <p>Keine Gewichtsdaten vorhanden.</p>
      </section>
    );
  }

  const {
    chartMin,
    chartMax,
    guideValues,
    points,
    linePath,
    hasLine,
  } = buildWeightLineChartModel(data);

  const displayedChartItem = displayedItem?.item ?? null;

  return (
    <section className="chart-section">
      <div className="chart-header">
        <h2>Gewichtsverlauf</h2>
        {usesUnboundedFallback && (
          <span className="weight-chart-note">Aktuellste Messungen</span>
        )}
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
              <span className="chart-guide-label">
                {formatWeightKg(value)}
              </span>
            </div>
          ))}

          <div className="weight-line-chart-plot">
            <svg
              className="weight-line-chart-svg"
              viewBox="0 0 100 100"
              preserveAspectRatio="none"
              aria-hidden="true"
            >
              {hasLine && (
                <path
                  className="weight-line-chart-path"
                  d={linePath}
                />
              )}
            </svg>

            <div className="weight-line-point-layer">
              {points.map((point) => {
                const isActive = displayedItem?.key === point.key;

                return (
                  <button
                    key={point.key}
                    type="button"
                    className={`weight-line-point ${
                      isActive ? 'is-active' : ''
                    }`}
                    style={{
                      left: `${point.x}%`,
                      top: `${point.y}%`,
                    }}
                    onMouseEnter={() => handleItemMouseEnter(point)}
                    onMouseLeave={handleItemMouseLeave}
                    onClick={() => handleItemClick(point)}
                    aria-label={`${getDetailsLabel(
                      point.item,
                      period
                    )}: ${formatWeightKg(point.item.weightKg)}`}
                  />
                );
              })}
            </div>
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
          className={`chart-details ${
            displayedChartItem ? '' : 'chart-details--empty'
          }`}
        >
          <div className="chart-details-label">
            {getDetailsLabel(displayedChartItem, period)}
          </div>

          <div className="chart-details-value">
            {getDetailsValue(displayedChartItem, period)}
          </div>

          <div
            className={`weight-chart-details-meta ${
              displayedChartItem ? '' : 'weight-chart-details-meta--empty'
            }`}
          >
            {getDetailsMeta(displayedChartItem, period)}
          </div>
        </div>
      </div>
    </section>
  );
}

export default WeightLineChart;