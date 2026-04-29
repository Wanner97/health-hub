function compareByDateAndMeasuredAtUtc(a, b) {
  const dateComparison = (a.date ?? '').localeCompare(b.date ?? '');

  if (dateComparison !== 0) {
    return dateComparison;
  }

  return (a.measuredAtUtc ?? '').localeCompare(b.measuredAtUtc ?? '');
}

export function sortWeightMeasurementsByDate(weightMeasurements) {
  if (!weightMeasurements?.length) {
    return [];
  }

  return [...weightMeasurements].sort(compareByDateAndMeasuredAtUtc);
}

export function sortWeightMeasurementsByDateDesc(weightMeasurements) {
  if (!weightMeasurements?.length) {
    return [];
  }

  return [...weightMeasurements].sort((a, b) =>
    compareByDateAndMeasuredAtUtc(b, a)
  );
}

function getMonthKey(dateString) {
  return dateString.slice(0, 7);
}

export function buildMonthlyWeightRows(weightMeasurements) {
  if (!weightMeasurements?.length) {
    return [];
  }

  const monthMap = new Map();

  for (const measurement of weightMeasurements) {
    const monthKey = getMonthKey(measurement.date);

    if (!monthMap.has(monthKey)) {
      monthMap.set(monthKey, {
        monthKey,
        totalWeightKg: 0,
        measurementCount: 0,
        minWeightKg: null,
        maxWeightKg: null,
        measurementDates: new Set(),
      });
    }

    const currentMonth = monthMap.get(monthKey);
    const currentWeightKg = measurement.weightKg ?? 0;

    currentMonth.totalWeightKg += currentWeightKg;
    currentMonth.measurementCount += 1;
    currentMonth.measurementDates.add(measurement.date);

    currentMonth.minWeightKg =
      currentMonth.minWeightKg == null
        ? currentWeightKg
        : Math.min(currentMonth.minWeightKg, currentWeightKg);

    currentMonth.maxWeightKg =
      currentMonth.maxWeightKg == null
        ? currentWeightKg
        : Math.max(currentMonth.maxWeightKg, currentWeightKg);
  }

  return [...monthMap.values()]
    .map((month) => ({
      monthKey: month.monthKey,
      averageWeightKg:
        month.measurementCount > 0
          ? month.totalWeightKg / month.measurementCount
          : 0,
      minWeightKg: month.minWeightKg ?? 0,
      maxWeightKg: month.maxWeightKg ?? 0,
      measurementCount: month.measurementCount,
      dayCount: month.measurementDates.size,
    }))
    .sort((a, b) => b.monthKey.localeCompare(a.monthKey));
}

export function takeLatestWeightMeasurements(weightMeasurements, limit) {
  if (!weightMeasurements?.length || limit <= 0) {
    return [];
  }

  return sortWeightMeasurementsByDateDesc(weightMeasurements)
    .slice(0, limit)
    .reverse();
}