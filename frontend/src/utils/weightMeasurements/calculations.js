export function calculateWeightMeasurementCount(weightMeasurements) {
  return weightMeasurements?.length ?? 0;
}

export function calculateAverageWeightKg(weightMeasurements) {
  if (!weightMeasurements?.length) {
    return 0;
  }

  const totalWeightKg = weightMeasurements.reduce(
    (sum, measurement) => sum + (measurement.weightKg ?? 0),
    0
  );

  return totalWeightKg / weightMeasurements.length;
}

export function calculateMinWeightKg(weightMeasurements) {
  if (!weightMeasurements?.length) {
    return 0;
  }

  return Math.min(
    ...weightMeasurements.map((measurement) => measurement.weightKg ?? 0)
  );
}

export function calculateMaxWeightKg(weightMeasurements) {
  if (!weightMeasurements?.length) {
    return 0;
  }

  return Math.max(
    ...weightMeasurements.map((measurement) => measurement.weightKg ?? 0)
  );
}