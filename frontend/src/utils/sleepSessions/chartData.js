import { PERIODS } from '../../constants/periods';
import { formatShortMonth } from '../date/dateFormatters';

function getDayOfMonthLabel(dateString) {
  const [, , day] = dateString.split('-');
  return day;
}

export function buildDailySleepChartData(rows) {
  if (!rows?.length) {
    return [];
  }

  return [...rows]
    .slice()
    .reverse()
    .map((row) => ({
      key: row.key,
      label: getDayOfMonthLabel(row.sleepDateEndKey),
      fullLabel: row.sleepDateStartKey,
      sleepDateStartKey: row.sleepDateStartKey,
      sleepDateEndKey: row.sleepDateEndKey,
      totalDurationMinutes: row.totalDurationMinutes ?? 0,
      sessionCount: row.sessionCount ?? 0,
      totalStageCount: row.totalStageCount ?? 0,
      awakeMinutes: row.awakeMinutes ?? 0,
      remMinutes: row.remMinutes ?? 0,
      lightMinutes: row.lightMinutes ?? 0,
      deepMinutes: row.deepMinutes ?? 0,
      awakePercentage: row.awakePercentage ?? null,
      remPercentage: row.remPercentage ?? null,
      lightPercentage: row.lightPercentage ?? null,
      deepPercentage: row.deepPercentage ?? null,
      startTimeUtc: row.startTimeUtc ?? null,
      endTimeUtc: row.endTimeUtc ?? null,
      timelineStartUtc: row.timelineStartUtc ?? null,
      timelineEndUtc: row.timelineEndUtc ?? null,
      sleepTimelineSegments: row.sleepTimelineSegments ?? [],
      sleepSessions: row.sleepSessions ?? [],
    }));
}

export function buildMonthlySleepChartData(rows) {
  if (!rows?.length) {
    return [];
  }

  return [...rows]
    .slice()
    .reverse()
    .map((month) => ({
      key: month.monthKey,
      label: formatShortMonth(month.monthKey),
      fullLabel: month.monthKey,
      monthKey: month.monthKey,
      averageSleepMinutes: month.averageSleepMinutes ?? 0,
      totalDurationMinutes: month.totalDurationMinutes ?? 0,
      sessionCount: month.sessionCount ?? 0,
      dayCount: month.dayCount ?? 0,
      awakeMinutes: month.awakeMinutes ?? 0,
      remMinutes: month.remMinutes ?? 0,
      lightMinutes: month.lightMinutes ?? 0,
      deepMinutes: month.deepMinutes ?? 0,
      averageAwakeMinutes: month.averageAwakeMinutes ?? 0,
      averageRemMinutes: month.averageRemMinutes ?? 0,
      averageLightMinutes: month.averageLightMinutes ?? 0,
      averageDeepMinutes: month.averageDeepMinutes ?? 0,
      awakePercentage: month.awakePercentage ?? null,
      remPercentage: month.remPercentage ?? null,
      lightPercentage: month.lightPercentage ?? null,
      deepPercentage: month.deepPercentage ?? null,
    }));
}

export function getSleepChartData(period, rows) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return buildMonthlySleepChartData(rows);
  }

  return buildDailySleepChartData(rows);
}