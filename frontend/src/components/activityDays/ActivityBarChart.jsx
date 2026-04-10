import { useState } from 'react';
import { PERIODS } from '../../utils/activityDays/periodUtils';
import {
  formatDate,
  formatMonthLabel,
  formatNumber,
} from '../../utils/activityDays/formatters';

function getNiceStep(maxValue) {
  if (maxValue <= 0) {
    return 1000;
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

function getDetailsLabel(item, period) {
  if (period === PERIODS.TWELVE_MONTHS) {
    const [year] = item.fullLabel.split('-');
    return `${formatMonthLabel(item.fullLabel)} ${year}`;
  }

  return formatDate(item.fullLabel);
}

function getDetailsValue(item, period) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return `Ø ${formatNumber(item.averageSteps)} Schritte / Tag`;
  }

  return `${formatNumber(item.steps)} Schritte`;
}

function ActivityBarChart({ period, data }) {
  const [activeItem, setActiveItem] = useState(null);

  if (!data?.length) {
    return (
      <section className="chart-section">
        <h2>Schrittverlauf</h2>
        <p>Keine Daten vorhanden.</p>
      </section>
    );
  }

  const valueKey = period === PERIODS.TWELVE_MONTHS ? 'averageSteps' : 'steps';
  const maxValue = Math.max(...data.map((item) => item[valueKey] ?? 0), 1);
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

  return (
    <section className="chart-section">
      <div className="chart-header">
        <h2>Schrittverlauf</h2>
      </div>

      <div className="chart-body">
        <div className="chart-canvas">
          {guideValues.map((value) => (
            <div
              key={value}
              className="chart-guide"
              style={{ bottom: `${(value / chartMax) * 100}%` }}
            >
              <span className="chart-guide-label">{formatNumber(value)}</span>
            </div>
          ))}

          <div className="bar-chart-columns">
            {data.map((item) => {
              const value = item[valueKey] ?? 0;
              const isActive = activeItem?.key === item.key;

              return (
                <div
                  key={item.key}
                  className={`bar-column-wrapper ${isActive ? 'is-active' : ''}`}
                  onMouseEnter={() => setActiveItem(item)}
                  onMouseLeave={() => setActiveItem(null)}
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
              {shouldShowLabel(index) ? item.label : ''}
            </div>
          ))}
        </div>

        {activeItem && (
          <div className="chart-details">
            <div className="chart-details-label">
              {getDetailsLabel(activeItem, period)}
            </div>
            <div className="chart-details-value">
              {getDetailsValue(activeItem, period)}
            </div>
          </div>
        )}
      </div>
    </section>
  );
}

export default ActivityBarChart;