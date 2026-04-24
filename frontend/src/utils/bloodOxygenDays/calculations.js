export function calculateAveragePercent(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return 0;
  }

  const totalMeasurements = bloodOxygenDays.reduce(
    (sum, day) => sum + (day.measurementCount ?? 0),
    0
  );

  if (totalMeasurements > 0) {
    const weightedSum = bloodOxygenDays.reduce(
      (sum, day) => sum + ((day.avgPercent ?? 0) * (day.measurementCount ?? 0)),
      0
    );

    return Math.round(weightedSum / totalMeasurements);
  }

  const averageSum = bloodOxygenDays.reduce(
    (sum, day) => sum + (day.avgPercent ?? 0),
    0
  );

  return Math.round(averageSum / bloodOxygenDays.length);
}

export function calculateMinPercent(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return 0;
  }

  return bloodOxygenDays.reduce((minValue, day) => {
    const currentValue = day.minPercent ?? minValue;
    return Math.min(minValue, currentValue);
  }, bloodOxygenDays[0]?.minPercent ?? 0);
}

export function calculateMaxPercent(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return 0;
  }

  return bloodOxygenDays.reduce((maxValue, day) => {
    const currentValue = day.maxPercent ?? maxValue;
    return Math.max(maxValue, currentValue);
  }, bloodOxygenDays[0]?.maxPercent ?? 0);
}

export function calculateTotalMeasurements(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return 0;
  }

  return bloodOxygenDays.reduce(
    (sum, day) => sum + (day.measurementCount ?? 0),
    0
  );
}