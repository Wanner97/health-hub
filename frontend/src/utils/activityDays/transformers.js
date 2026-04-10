import { addDays, formatDateForInput, parseDateString } from './periodUtils';

export function sortActivityDaysByDate(activityDays) {
  if (!activityDays?.length) {
    return [];
  }

  return [...activityDays].sort((a, b) => a.date.localeCompare(b.date));
}

export function sortActivityDaysByDateDesc(activityDays) {
  if (!activityDays?.length) {
    return [];
  }

  return [...activityDays].sort((a, b) => b.date.localeCompare(a.date));
}

export function fillMissingDays(activityDays, fromString, toString) {
  const byDate = new Map(activityDays.map((day) => [day.date, day]));
  const result = [];

  let current = parseDateString(fromString);
  const end = parseDateString(toString);

  while (current <= end) {
    const currentKey = formatDateForInput(current);

    result.push(
      byDate.get(currentKey) ?? {
        date: currentKey,
        steps: 0,
        distanceMeters: 0,
        startTimeUtc: null,
        endTimeUtc: null,
      }
    );

    current = addDays(current, 1);
  }

  return result;
}

export function getMonthKey(dateString) {
  return dateString.slice(0, 7);
}

export function buildMonthlyAverageRows(activityDays) {
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

  return [...monthMap.values()]
    .map((month) => ({
      monthKey: month.monthKey,
      averageSteps: Math.round(month.totalSteps / month.dayCount),
      averageDistanceMeters: month.totalDistanceMeters / month.dayCount,
      totalSteps: month.totalSteps,
      dayCount: month.dayCount,
    }))
    .sort((a, b) => b.monthKey.localeCompare(a.monthKey));
}