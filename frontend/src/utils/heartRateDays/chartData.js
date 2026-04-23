import { PERIODS } from '../../constants/periods';
import { formatShortMonth } from '../date/dateFormatters';
import { buildMonthlyHeartRateRows } from './transformers';

function getDayLabel(dateString) {
  const [, , day] = dateString.split('-');
  return day;
}

export function buildDailyHeartRateChartData(heartRateDays) {
  if (!heartRateDays?.length) {
    return [];
  }

  return heartRateDays.map((day) => ({
    key: day.date,
    label: getDayLabel(day.date),
    fullLabel: day.date,
    avgBpm: day.avgBpm ?? 0,
    minBpm: day.minBpm ?? 0,
    maxBpm: day.maxBpm ?? 0,
    measurementCount: day.measurementCount ?? 0,
    hourlyRecords: day.hourlyRecords ?? [],
  }));
}

export function buildMonthlyHeartRateChartData(heartRateDays) {
  if (!heartRateDays?.length) {
    return [];
  }

  return buildMonthlyHeartRateRows(heartRateDays)
    .slice()
    .sort((a, b) => a.monthKey.localeCompare(b.monthKey))
    .map((month) => ({
      key: month.monthKey,
      label: formatShortMonth(month.monthKey),
      fullLabel: month.monthKey,
      averageBpm: month.averageBpm ?? 0,
      minBpm: month.minBpm ?? 0,
      maxBpm: month.maxBpm ?? 0,
      measurementCount: month.measurementCount ?? 0,
      dayCount: month.dayCount ?? 0,
    }));
}

export function getHeartRateChartData(period, heartRateDays) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return buildMonthlyHeartRateChartData(heartRateDays);
  }

  return buildDailyHeartRateChartData(heartRateDays);
}