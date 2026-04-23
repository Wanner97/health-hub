export function sortHeartRateDaysByDate(heartRateDays) {
  if (!heartRateDays?.length) {
    return [];
  }

  return [...heartRateDays].sort((a, b) => a.date.localeCompare(b.date));
}

export function sortHeartRateDaysByDateDesc(heartRateDays) {
  if (!heartRateDays?.length) {
    return [];
  }

  return [...heartRateDays].sort((a, b) => b.date.localeCompare(a.date));
}

function getMonthKey(dateString) {
  return dateString.slice(0, 7);
}

export function buildMonthlyHeartRateRows(heartRateDays) {
  if (!heartRateDays?.length) {
    return [];
  }

  const monthMap = new Map();

  for (const day of heartRateDays) {
    const monthKey = getMonthKey(day.date);

    if (!monthMap.has(monthKey)) {
      monthMap.set(monthKey, {
        monthKey,
        weightedBpmSum: 0,
        fallbackBpmSum: 0,
        measurementCount: 0,
        minBpm: null,
        maxBpm: null,
        dayCount: 0,
      });
    }

    const currentMonth = monthMap.get(monthKey);
    const currentAvgBpm = day.avgBpm ?? 0;
    const currentMeasurementCount = day.measurementCount ?? 0;
    const currentMinBpm = day.minBpm ?? null;
    const currentMaxBpm = day.maxBpm ?? null;

    currentMonth.weightedBpmSum += currentAvgBpm * currentMeasurementCount;
    currentMonth.fallbackBpmSum += currentAvgBpm;
    currentMonth.measurementCount += currentMeasurementCount;
    currentMonth.dayCount += 1;

    if (currentMinBpm != null) {
      currentMonth.minBpm =
        currentMonth.minBpm == null
          ? currentMinBpm
          : Math.min(currentMonth.minBpm, currentMinBpm);
    }

    if (currentMaxBpm != null) {
      currentMonth.maxBpm =
        currentMonth.maxBpm == null
          ? currentMaxBpm
          : Math.max(currentMonth.maxBpm, currentMaxBpm);
    }
  }

  return [...monthMap.values()]
    .map((month) => {
      const averageBpm =
        month.measurementCount > 0
          ? Math.round(month.weightedBpmSum / month.measurementCount)
          : Math.round(month.fallbackBpmSum / month.dayCount);

      return {
        monthKey: month.monthKey,
        averageBpm,
        minBpm: month.minBpm ?? 0,
        maxBpm: month.maxBpm ?? 0,
        measurementCount: month.measurementCount,
        dayCount: month.dayCount,
      };
    })
    .sort((a, b) => b.monthKey.localeCompare(a.monthKey));
}