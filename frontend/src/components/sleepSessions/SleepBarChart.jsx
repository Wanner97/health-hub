import { PERIODS } from '../../constants/periods';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import {
  buildGuideValues,
  getChartMax,
} from '../../utils/charts/scaleUtils';
import { shouldShowChartLabel } from '../../utils/charts/labelUtils';
import { useSelectableChartItem } from '../../hooks/useSelectableChartItem';
import SleepPhaseDetails from './SleepPhaseDetails';

function getValueForItem(item, period) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return item.averageSleepMinutes ?? 0;
  }

  return item.totalDurationMinutes ?? 0;
}

function buildDailySegments(item) {
  const totalDurationMinutes = item.totalDurationMinutes ?? 0;

  const awakeMinutes = item.awakeMinutes ?? 0;
  const remMinutes = item.remMinutes ?? 0;
  const lightMinutes = item.lightMinutes ?? 0;
  const deepMinutes = item.deepMinutes ?? 0;

  const knownMinutes =
    awakeMinutes + remMinutes + lightMinutes + deepMinutes;

  const unknownMinutes = Math.max(0, totalDurationMinutes - knownMinutes);

  return [
    {
      key: 'awake',
      minutes: awakeMinutes,
      className: 'sleep-bar-segment--awake',
    },
    {
      key: 'rem',
      minutes: remMinutes,
      className: 'sleep-bar-segment--rem',
    },
    {
      key: 'light',
      minutes: lightMinutes,
      className: 'sleep-bar-segment--light',
    },
    {
      key: 'deep',
      minutes: deepMinutes,
      className: 'sleep-bar-segment--deep',
    },
    {
      key: 'unknown',
      minutes: unknownMinutes,
      className: 'sleep-bar-segment--unknown',
    },
  ].filter((segment) => segment.minutes > 0);
}

function SleepBarChart({ period, data }) {
  const {
    displayedItem,
    handleItemMouseEnter,
    handleItemMouseLeave,
    handleItemClick,
  } = useSelectableChartItem({
    resetSelectors: ['.sleep-bar-column-wrapper', '.sleep-chart-details'],
  });

  if (!data?.length) {
    return (
      <section className="chart-section">
        <h2>Schlafverlauf</h2>
        <p>Keine Daten vorhanden.</p>
      </section>
    );
  }

  const maxValue = Math.max(...data.map((item) => getValueForItem(item, period)), 1);
  const chartMax = getChartMax(maxValue, 60);
  const guideValues = buildGuideValues(chartMax, 60);

  function getBarHeight(value) {
    return `${(value / chartMax) * 100}%`;
  }

  return (
    <section className="chart-section sleep-chart-section">
      <div className="chart-header">
        <h2>Schlafverlauf</h2>
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
                {formatDurationMinutes(value)}
              </span>
            </div>
          ))}

          <div className="bar-chart-columns">
            {data.map((item) => {
              const totalValue = getValueForItem(item, period);
              const isActive = displayedItem?.key === item.key;

              if (period === PERIODS.TWELVE_MONTHS) {
                return (
                  <div
                    key={item.key}
                    className={`bar-column-wrapper sleep-bar-column-wrapper ${
                      isActive ? 'is-active' : ''
                    }`}
                    onMouseEnter={() => handleItemMouseEnter(item)}
                    onMouseLeave={handleItemMouseLeave}
                    onClick={() => handleItemClick(item)}
                  >
                    <div
                      className="bar-fill sleep-bar-fill--monthly"
                      style={{ height: getBarHeight(totalValue) }}
                    />
                  </div>
                );
              }

              const segments = buildDailySegments(item);

              return (
                <div
                  key={item.key}
                  className={`bar-column-wrapper sleep-bar-column-wrapper ${
                    isActive ? 'is-active' : ''
                  }`}
                  onMouseEnter={() => handleItemMouseEnter(item)}
                  onMouseLeave={handleItemMouseLeave}
                  onClick={() => handleItemClick(item)}
                >
                  <div
                    className="sleep-bar-fill-stack"
                    style={{ height: getBarHeight(totalValue) }}
                  >
                    {segments.map((segment) => (
                      <div
                        key={segment.key}
                        className={`sleep-bar-segment ${segment.className}`}
                        style={{
                          height: `${(segment.minutes / totalValue) * 100}%`,
                        }}
                      />
                    ))}
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

        <SleepPhaseDetails item={displayedItem} period={period} />
      </div>
    </section>
  );
}

export default SleepBarChart;