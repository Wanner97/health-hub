import { PERIODS } from '../../constants/periods';
import { formatShortMonth } from '../date/dateFormatters';
import { buildMonthlyActivityAggregates } from './monthlyAggregates';

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

  return buildMonthlyActivityAggregates(activityDays)
    .sort((a, b) => a.monthKey.localeCompare(b.monthKey))
    .map((month) => ({
      key: month.monthKey,
      label: formatShortMonth(month.monthKey),
      fullLabel: month.monthKey,
      averageSteps: month.averageSteps,
      averageDistanceMeters: month.averageDistanceMeters,
      totalSteps: month.totalSteps,
      totalDistanceMeters: month.totalDistanceMeters,
      dayCount: month.dayCount,
    }));
}

export function getChartData(period, activityDays) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return buildMonthlyChartData(activityDays);
  }

  return buildDailyChartData(activityDays);
}