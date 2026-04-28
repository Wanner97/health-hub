import { getNiceStep } from './scaleUtils';

export function buildRangeScale(minValue, maxValue, options = {}) {
  const {
    padding = 0,
    minLimit = 0,
    maxLimit = Number.POSITIVE_INFINITY,
    fallbackStep = 20,
  } = options;

  const paddedMin = Math.max(minLimit, (minValue ?? 0) - padding);
  const paddedMax = Math.min(maxLimit, (maxValue ?? 0) + padding);
  const step = getNiceStep(Math.max(paddedMax - paddedMin, 1), fallbackStep);

  const chartMin = Math.max(minLimit, Math.floor(paddedMin / step) * step);
  const chartMax = Math.min(maxLimit, Math.ceil(paddedMax / step) * step);

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

export function getRangePosition(value, chartMin, chartMax) {
  if (value == null || chartMax <= chartMin) {
    return 0;
  }

  return ((value - chartMin) / (chartMax - chartMin)) * 100;
}