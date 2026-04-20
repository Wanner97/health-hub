export function getNiceStep(maxValue, fallbackStep = 1000) {
  if (maxValue <= 0) {
    return fallbackStep;
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

export function getChartMax(maxValue, fallbackStep = 1000) {
  const step = getNiceStep(maxValue, fallbackStep);
  return Math.ceil(maxValue / step) * step;
}

export function buildGuideValues(chartMax, fallbackStep = 1000) {
  const step = getNiceStep(chartMax, fallbackStep);
  const values = [];

  for (let value = 0; value <= chartMax; value += step) {
    values.push(value);
  }

  return values;
}