import {
  addDays,
  formatDateForInput,
  parseDateString,
} from '../date/dateHelpers';
import { buildMonthlyActivityAggregates } from './monthlyAggregates';

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
        distanceSource: null,
        totalCaloriesBurnedKcal: 0,
        startTimeUtc: null,
        endTimeUtc: null,
      }
    );

    current = addDays(current, 1);
  }

  return result;
}

export function buildMonthlyAverageRows(activityDays) {
  if (!activityDays?.length) {
    return [];
  }

  return buildMonthlyActivityAggregates(activityDays).sort((a, b) =>
    b.monthKey.localeCompare(a.monthKey)
  );
}