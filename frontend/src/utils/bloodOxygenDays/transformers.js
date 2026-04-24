export function sortBloodOxygenDaysByDate(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return [];
  }

  return [...bloodOxygenDays].sort((a, b) => a.date.localeCompare(b.date));
}

export function sortBloodOxygenDaysByDateDesc(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return [];
  }

  return [...bloodOxygenDays].sort((a, b) => b.date.localeCompare(a.date));
}

function getMonthKey(dateString) {
  return dateString.slice(0, 7);
}

export function buildMonthlyBloodOxygenRows(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return [];
  }

  const monthMap = new Map();

  for (const day of bloodOxygenDays) {
    const monthKey = getMonthKey(day.date);

    if (!monthMap.has(monthKey)) {
      monthMap.set(monthKey, {
        monthKey,
        weightedPercentSum: 0,
        fallbackPercentSum: 0,
        measurementCount: 0,
        minPercent: null,
        maxPercent: null,
        dayCount: 0,
      });
    }

    const currentMonth = monthMap.get(monthKey);
    const currentAvgPercent = day.avgPercent ?? 0;
    const currentMeasurementCount = day.measurementCount ?? 0;
    const currentMinPercent = day.minPercent ?? null;
    const currentMaxPercent = day.maxPercent ?? null;

    currentMonth.weightedPercentSum += currentAvgPercent * currentMeasurementCount;
    currentMonth.fallbackPercentSum += currentAvgPercent;
    currentMonth.measurementCount += currentMeasurementCount;
    currentMonth.dayCount += 1;

    if (currentMinPercent != null) {
      currentMonth.minPercent =
        currentMonth.minPercent == null
          ? currentMinPercent
          : Math.min(currentMonth.minPercent, currentMinPercent);
    }

    if (currentMaxPercent != null) {
      currentMonth.maxPercent =
        currentMonth.maxPercent == null
          ? currentMaxPercent
          : Math.max(currentMonth.maxPercent, currentMaxPercent);
    }
  }

  return [...monthMap.values()]
    .map((month) => {
      const averagePercent =
        month.measurementCount > 0
          ? Math.round(month.weightedPercentSum / month.measurementCount)
          : Math.round(month.fallbackPercentSum / month.dayCount);

      return {
        monthKey: month.monthKey,
        averagePercent,
        minPercent: month.minPercent ?? 0,
        maxPercent: month.maxPercent ?? 0,
        measurementCount: month.measurementCount,
        dayCount: month.dayCount,
      };
    })
    .sort((a, b) => b.monthKey.localeCompare(a.monthKey));
}