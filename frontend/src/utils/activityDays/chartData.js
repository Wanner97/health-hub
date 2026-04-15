import { PERIODS } from '../../constants/periods';
import { formatShortMonth } from '../date/dateFormatters';

function getDayOfMonthLabel(dateString) {
  const [, , day] = dateString.split('-');
  return day;
}

export function buildDailyChartData(activityDays) {
  if (!activityDays?.length) {
    return [];
  }

  return activityDays.map((day) => ({
    key: day.date,
    label: getDayOfMonthLabel(day.date),
    fullLabel: day.date,
    steps: day.steps ?? 0,
    distanceMeters: day.distanceMeters ?? 0,
  }));
}

export function buildMonthlyChartData(activityDays) {
  if (!activityDays?.length) {
    return [];
  }

  const monthMap = new Map();

  for (const day of activityDays) {
    const monthKey = day.date.slice(0, 7);

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
    .sort((a, b) => a.monthKey.localeCompare(b.monthKey))
    .map((month) => ({
      key: month.monthKey,
      label: formatShortMonth(month.monthKey),
      fullLabel: month.monthKey,
      averageSteps: Math.round(month.totalSteps / month.dayCount),
      averageDistanceMeters: month.totalDistanceMeters / month.dayCount,
      totalSteps: month.totalSteps,
      dayCount: month.dayCount,
    }));
}

export function getChartData(period, activityDays) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return buildMonthlyChartData(activityDays);
  }

  return buildDailyChartData(activityDays);
}