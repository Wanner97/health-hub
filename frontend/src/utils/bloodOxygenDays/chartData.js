import { PERIODS } from '../../constants/periods';
import { formatShortMonth } from '../date/dateFormatters';
import { buildMonthlyBloodOxygenRows } from './transformers';

function getDayLabel(dateString) {
  const [, , day] = dateString.split('-');
  return day;
}

export function buildDailyBloodOxygenChartData(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return [];
  }

  return bloodOxygenDays.map((day) => ({
    key: day.date,
    label: getDayLabel(day.date),
    fullLabel: day.date,
    avgPercent: day.avgPercent ?? 0,
    minPercent: day.minPercent ?? 0,
    maxPercent: day.maxPercent ?? 0,
    measurementCount: day.measurementCount ?? 0,
  }));
}

export function buildMonthlyBloodOxygenChartData(bloodOxygenDays) {
  if (!bloodOxygenDays?.length) {
    return [];
  }

  return buildMonthlyBloodOxygenRows(bloodOxygenDays)
    .slice()
    .sort((a, b) => a.monthKey.localeCompare(b.monthKey))
    .map((month) => ({
      key: month.monthKey,
      label: formatShortMonth(month.monthKey),
      fullLabel: month.monthKey,
      averagePercent: month.averagePercent ?? 0,
      minPercent: month.minPercent ?? 0,
      maxPercent: month.maxPercent ?? 0,
      measurementCount: month.measurementCount ?? 0,
      dayCount: month.dayCount ?? 0,
    }));
}

export function getBloodOxygenChartData(period, bloodOxygenDays) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return buildMonthlyBloodOxygenChartData(bloodOxygenDays);
  }

  return buildDailyBloodOxygenChartData(bloodOxygenDays);
}