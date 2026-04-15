import { addDays, formatDateForInput } from '../date/dateHelpers';
import {
  calculatePercentage,
  calculateStageDurationMinutes,
} from './calculations';

function getSleepWindowStartDate(startTimeUtc) {
  const localStart = new Date(`${startTimeUtc}Z`);
  const windowStart = new Date(
    localStart.getFullYear(),
    localStart.getMonth(),
    localStart.getDate()
  );

  if (localStart.getHours() < 12) {
    windowStart.setDate(windowStart.getDate() - 1);
  }

  return windowStart;
}

function addStageMinutes(target, stage) {
  const stageMinutes = calculateStageDurationMinutes(stage);

  if (stage.stage === 'awake') {
    target.awakeMinutes += stageMinutes;
  }

  if (stage.stage === 'light') {
    target.lightMinutes += stageMinutes;
  }

  if (stage.stage === 'deep') {
    target.deepMinutes += stageMinutes;
  }

  if (stage.stage === 'rem') {
    target.remMinutes += stageMinutes;
  }
}

function buildPercentages(row) {
  const totalRecordedStageMinutes =
    row.awakeMinutes +
    row.lightMinutes +
    row.deepMinutes +
    row.remMinutes;

  return {
    ...row,
    awakePercentage: calculatePercentage(row.awakeMinutes, totalRecordedStageMinutes),
    lightPercentage: calculatePercentage(row.lightMinutes, totalRecordedStageMinutes),
    deepPercentage: calculatePercentage(row.deepMinutes, totalRecordedStageMinutes),
    remPercentage: calculatePercentage(row.remMinutes, totalRecordedStageMinutes),
  };
}

export function buildSleepDayRows(sessions) {
  if (!sessions?.length) {
    return [];
  }

  const dayMap = new Map();

  for (const session of sessions) {
    if (!session?.startTimeUtc || !session?.endTimeUtc) {
      continue;
    }

    const windowStartDate = getSleepWindowStartDate(session.startTimeUtc);
    const windowEndDate = addDays(windowStartDate, 1);

    const sleepDateStartKey = formatDateForInput(windowStartDate);
    const sleepDateEndKey = formatDateForInput(windowEndDate);
    const rowKey = sleepDateStartKey;

    if (!dayMap.has(rowKey)) {
      dayMap.set(rowKey, {
        key: rowKey,
        sleepDateStartKey,
        sleepDateEndKey,
        startTimeUtc: session.startTimeUtc,
        endTimeUtc: session.endTimeUtc,
        totalDurationMinutes: 0,
        sessionCount: 0,
        totalStageCount: 0,
        awakeMinutes: 0,
        lightMinutes: 0,
        deepMinutes: 0,
        remMinutes: 0,
      });
    }

    const currentDay = dayMap.get(rowKey);

    if (
      new Date(`${session.startTimeUtc}Z`) <
      new Date(`${currentDay.startTimeUtc}Z`)
    ) {
      currentDay.startTimeUtc = session.startTimeUtc;
    }

    if (
      new Date(`${session.endTimeUtc}Z`) >
      new Date(`${currentDay.endTimeUtc}Z`)
    ) {
      currentDay.endTimeUtc = session.endTimeUtc;
    }

    currentDay.totalDurationMinutes += session.durationMinutes ?? 0;
    currentDay.sessionCount += 1;

    const stages = session.sleepStages ?? [];
    currentDay.totalStageCount += stages.length;

    for (const stage of stages) {
      addStageMinutes(currentDay, stage);
    }
  }

  return [...dayMap.values()]
    .map(buildPercentages)
    .sort((a, b) => b.sleepDateStartKey.localeCompare(a.sleepDateStartKey));
}

export function buildMonthlySleepRows(sleepDayRows) {
  if (!sleepDayRows?.length) {
    return [];
  }

  const monthMap = new Map();

  for (const row of sleepDayRows) {
    const monthKey = row.sleepDateStartKey.slice(0, 7);

    if (!monthMap.has(monthKey)) {
      monthMap.set(monthKey, {
        key: monthKey,
        monthKey,
        totalDurationMinutes: 0,
        dayCount: 0,
        sessionCount: 0,
        totalStageCount: 0,
        awakeMinutes: 0,
        lightMinutes: 0,
        deepMinutes: 0,
        remMinutes: 0,
      });
    }

    const currentMonth = monthMap.get(monthKey);

    currentMonth.totalDurationMinutes += row.totalDurationMinutes ?? 0;
    currentMonth.dayCount += 1;
    currentMonth.sessionCount += row.sessionCount ?? 0;
    currentMonth.totalStageCount += row.totalStageCount ?? 0;
    currentMonth.awakeMinutes += row.awakeMinutes ?? 0;
    currentMonth.lightMinutes += row.lightMinutes ?? 0;
    currentMonth.deepMinutes += row.deepMinutes ?? 0;
    currentMonth.remMinutes += row.remMinutes ?? 0;
  }

  return [...monthMap.values()]
    .map((month) =>
      buildPercentages({
        ...month,
        averageSleepMinutes: Math.round(
          month.totalDurationMinutes / month.dayCount
        ),
      })
    )
    .sort((a, b) => b.monthKey.localeCompare(a.monthKey));
}