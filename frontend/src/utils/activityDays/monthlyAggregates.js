export function getMonthKey(dateString) {
  return dateString.slice(0, 7);
}

export function buildMonthlyActivityAggregates(activityDays) {
  if (!activityDays?.length) {
    return [];
  }

  const monthMap = new Map();

  for (const day of activityDays) {
    const monthKey = getMonthKey(day.date);

    if (!monthMap.has(monthKey)) {
      monthMap.set(monthKey, {
        monthKey,
        totalSteps: 0,
        totalDistanceMeters: 0,
        dayCount: 0,
      });
    }

    const currentMonth = monthMap.get(monthKey);

    currentMonth.totalSteps += day.steps ?? 0;
    currentMonth.totalDistanceMeters += day.distanceMeters ?? 0;
    currentMonth.dayCount += 1;
  }

  return [...monthMap.values()].map((month) => ({
    monthKey: month.monthKey,
    totalSteps: month.totalSteps,
    totalDistanceMeters: month.totalDistanceMeters,
    dayCount: month.dayCount,
    averageSteps: Math.round(month.totalSteps / month.dayCount),
    averageDistanceMeters: month.totalDistanceMeters / month.dayCount,
  }));
}