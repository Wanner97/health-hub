import { useEffect, useMemo, useState } from 'react';
import { PERIODS } from '../../constants/periods';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import SleepPhaseDetails from './SleepPhaseDetails';

function getNiceStep(maxValue) {
  if (maxValue <= 0) {
    return 60;
  }

  const roughStep = maxValue / 4;
  const magnitude = Math.pow(10, Math.floor(Math.log10(roughStep)));
  const normalized = roughStep / magnitude;

  let niceNormalized;

  if (normalized <= 1) {
    niceNormalized = 1;
  } else if (normalized <= 2) {
    niceNormalized = 2;
  } else if (normalized <= 5) {
    niceNormalized = 5;
  } else {
    niceNormalized = 10;
  }

  return niceNormalized * magnitude;
}

function getChartMax(maxValue) {
  const step = getNiceStep(maxValue);
  return Math.ceil(maxValue / step) * step;
}

function buildGuideValues(chartMax) {
  const step = getNiceStep(chartMax);
  const values = [];

  for (let value = 0; value <= chartMax; value += step) {
    values.push(value);
  }

  return values;
}

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

function ActivityBarChart({ period, data }) {
  const [hoveredItem, setHoveredItem] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);

  const displayedItem = useMemo(() => {
    return hoveredItem ?? selectedItem;
  }, [hoveredItem, selectedItem]);

  useEffect(() => {
    function handleDocumentMouseDown(event) {
      const clickedBar = event.target.closest('.sleep-bar-column-wrapper');
      const clickedDetails = event.target.closest('.sleep-chart-details');

      if (!clickedBar && !clickedDetails) {
        setHoveredItem(null);
        setSelectedItem(null);
      }
    }

    document.addEventListener('mousedown', handleDocumentMouseDown);

    return () => {
      document.removeEventListener('mousedown', handleDocumentMouseDown);
    };
  }, []);

  if (!data?.length) {
    return (
      <section className="chart-section">
        <h2>Schlafverlauf</h2>
        <p>Keine Daten vorhanden.</p>
      </section>
    );
  }

  const maxValue = Math.max(...data.map((item) => getValueForItem(item, period)), 1);
  const chartMax = getChartMax(maxValue);
  const guideValues = buildGuideValues(chartMax);

  function getBarHeight(value) {
    return `${(value / chartMax) * 100}%`;
  }

  function shouldShowLabel(index) {
    if (period === PERIODS.SEVEN_DAYS) {
      return true;
    }

    if (period === PERIODS.THIRTY_ONE_DAYS) {
      return index % 3 === 0 || index === data.length - 1;
    }

    return true;
  }

  function handleBarClick(item) {
    setSelectedItem((currentSelectedItem) => {
      if (currentSelectedItem?.key === item.key) {
        return null;
      }

      return item;
    });
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
                    className={`bar-column-wrapper sleep-bar-column-wrapper ${isActive ? 'is-active' : ''}`}
                    onMouseEnter={() => setHoveredItem(item)}
                    onMouseLeave={() => setHoveredItem(null)}
                    onClick={() => handleBarClick(item)}
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
                  className={`bar-column-wrapper sleep-bar-column-wrapper ${isActive ? 'is-active' : ''}`}
                  onMouseEnter={() => setHoveredItem(item)}
                  onMouseLeave={() => setHoveredItem(null)}
                  onClick={() => handleBarClick(item)}
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
              {shouldShowLabel(index) ? item.label : ''}
            </div>
          ))}
        </div>

        <SleepPhaseDetails item={displayedItem} period={period} />
      </div>
    </section>
  );
}

export default ActivityBarChart;