export function calculateAverageBpm(heartRateDays) {
  if (!heartRateDays?.length) {
    return 0;
  }

  const totalMeasurements = heartRateDays.reduce(
    (sum, day) => sum + (day.measurementCount ?? 0),
    0
  );

  if (totalMeasurements > 0) {
    const weightedSum = heartRateDays.reduce(
      (sum, day) => sum + ((day.avgBpm ?? 0) * (day.measurementCount ?? 0)),
      0
    );

    return Math.round(weightedSum / totalMeasurements);
  }

  const averageSum = heartRateDays.reduce(
    (sum, day) => sum + (day.avgBpm ?? 0),
    0
  );

  return Math.round(averageSum / heartRateDays.length);
}

export function calculateMinBpm(heartRateDays) {
  if (!heartRateDays?.length) {
    return 0;
  }

  return heartRateDays.reduce((minValue, day) => {
    const currentValue = day.minBpm ?? minValue;
    return Math.min(minValue, currentValue);
  }, heartRateDays[0]?.minBpm ?? 0);
}

export function calculateMaxBpm(heartRateDays) {
  if (!heartRateDays?.length) {
    return 0;
  }

  return heartRateDays.reduce((maxValue, day) => {
    const currentValue = day.maxBpm ?? maxValue;
    return Math.max(maxValue, currentValue);
  }, heartRateDays[0]?.maxBpm ?? 0);
}

export function calculateTotalMeasurements(heartRateDays) {
  if (!heartRateDays?.length) {
    return 0;
  }

  return heartRateDays.reduce(
    (sum, day) => sum + (day.measurementCount ?? 0),
    0
  );
}