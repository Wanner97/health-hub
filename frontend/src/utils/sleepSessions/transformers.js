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

function normalizeStage(stage) {
  if (!stage?.startTimeUtc || !stage?.endTimeUtc || !stage?.stage) {
    return null;
  }

  const startMs = new Date(`${stage.startTimeUtc}Z`).getTime();
  const endMs = new Date(`${stage.endTimeUtc}Z`).getTime();

  if (Number.isNaN(startMs) || Number.isNaN(endMs) || endMs <= startMs) {
    return null;
  }

  return {
    stage: stage.stage,
    startTimeUtc: stage.startTimeUtc,
    endTimeUtc: stage.endTimeUtc,
    startMs,
    endMs,
    durationMinutes: calculateStageDurationMinutes(stage),
  };
}

function normalizeSession(session, index) {
  const startMs = new Date(`${session.startTimeUtc}Z`).getTime();
  const endMs = new Date(`${session.endTimeUtc}Z`).getTime();

  const normalizedStages = (session.sleepStages ?? [])
    .map(normalizeStage)
    .filter(Boolean)
    .sort((a, b) => a.startMs - b.startMs);

  return {
    ...session,
    __id: `${session.startTimeUtc}-${session.endTimeUtc}-${index}`,
    __startMs: startMs,
    __endMs: endMs,
    __durationMinutes: session.durationMinutes ?? 0,
    __stageCount: normalizedStages.length,
    __hasStageData: normalizedStages.length > 0 ? 1 : 0,
    __stages: normalizedStages,
  };
}

function compareSessionPriority(a, b) {
  if (b.__hasStageData !== a.__hasStageData) {
    return b.__hasStageData - a.__hasStageData;
  }

  if (b.__stageCount !== a.__stageCount) {
    return b.__stageCount - a.__stageCount;
  }

  if (b.__durationMinutes !== a.__durationMinutes) {
    return b.__durationMinutes - a.__durationMinutes;
  }

  return b.__startMs - a.__startMs;
}

function sessionsOverlap(a, b) {
  return a.__startMs < b.__endMs && b.__startMs < a.__endMs;
}

function stripInternalSessionFields(session) {
  const {
    __id,
    __startMs,
    __endMs,
    __durationMinutes,
    __stageCount,
    __hasStageData,
    __stages,
    ...cleanSession
  } = session;

  return cleanSession;
}

function selectPrimarySessions(sessions) {
  const normalizedSessions = sessions
    .map(normalizeSession)
    .sort(compareSessionPriority);

  const selectedSessions = [];

  for (const session of normalizedSessions) {
    const overlapsWithSelected = selectedSessions.some((selectedSession) =>
      sessionsOverlap(selectedSession, session)
    );

    if (!overlapsWithSelected) {
      selectedSessions.push(session);
    }
  }

  return selectedSessions.sort((a, b) => a.__startMs - b.__startMs);
}

function buildTimelineSegments(sessions) {
  const segments = sessions
    .flatMap((session) => session.__stages)
    .sort((a, b) => a.startMs - b.startMs);

  if (!segments.length) {
    return [];
  }

  const mergedSegments = [];

  for (const segment of segments) {
    const lastSegment = mergedSegments[mergedSegments.length - 1];

    if (
      lastSegment &&
      lastSegment.stage === segment.stage &&
      segment.startMs <= lastSegment.endMs
    ) {
      lastSegment.endMs = Math.max(lastSegment.endMs, segment.endMs);
      lastSegment.endTimeUtc = new Date(lastSegment.endMs).toISOString();
      lastSegment.durationMinutes = Math.max(
        0,
        Math.round((lastSegment.endMs - lastSegment.startMs) / 60000)
      );

      continue;
    }

    mergedSegments.push({
      stage: segment.stage,
      startTimeUtc: segment.startTimeUtc,
      endTimeUtc: segment.endTimeUtc,
      startMs: segment.startMs,
      endMs: segment.endMs,
      durationMinutes: segment.durationMinutes,
    });
  }

  return mergedSegments;
}

function buildPercentages(row) {
  const totalRecordedStageMinutes =
    row.awakeMinutes +
    row.lightMinutes +
    row.deepMinutes +
    row.remMinutes;

  return {
    ...row,
    awakePercentage: calculatePercentage(
      row.awakeMinutes,
      totalRecordedStageMinutes
    ),
    lightPercentage: calculatePercentage(
      row.lightMinutes,
      totalRecordedStageMinutes
    ),
    deepPercentage: calculatePercentage(
      row.deepMinutes,
      totalRecordedStageMinutes
    ),
    remPercentage: calculatePercentage(
      row.remMinutes,
      totalRecordedStageMinutes
    ),
  };
}

function buildDayRow(day) {
  const selectedSessions = selectPrimarySessions(day.sleepSessions);
  const timelineSegments = buildTimelineSegments(selectedSessions);

  const result = {
    key: day.key,
    sleepDateStartKey: day.sleepDateStartKey,
    sleepDateEndKey: day.sleepDateEndKey,
    startTimeUtc: selectedSessions[0]?.startTimeUtc ?? day.startTimeUtc,
    endTimeUtc:
      selectedSessions[selectedSessions.length - 1]?.endTimeUtc ?? day.endTimeUtc,
    totalDurationMinutes: selectedSessions.reduce(
      (sum, session) => sum + (session.durationMinutes ?? 0),
      0
    ),
    sessionCount: selectedSessions.length,
    totalStageCount: selectedSessions.reduce(
      (sum, session) => sum + session.__stageCount,
      0
    ),
    awakeMinutes: 0,
    lightMinutes: 0,
    deepMinutes: 0,
    remMinutes: 0,
    sleepSessions: selectedSessions.map(stripInternalSessionFields),
    sleepTimelineSegments: timelineSegments,
    timelineStartUtc: timelineSegments[0]?.startTimeUtc ?? null,
    timelineEndUtc:
      timelineSegments[timelineSegments.length - 1]?.endTimeUtc ?? null,
  };

  for (const segment of timelineSegments) {
    if (segment.stage === 'awake') {
      result.awakeMinutes += segment.durationMinutes;
    }

    if (segment.stage === 'light') {
      result.lightMinutes += segment.durationMinutes;
    }

    if (segment.stage === 'deep') {
      result.deepMinutes += segment.durationMinutes;
    }

    if (segment.stage === 'rem') {
      result.remMinutes += segment.durationMinutes;
    }
  }

  return buildPercentages(result);
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
        sleepSessions: [],
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

    currentDay.sleepSessions.push(session);
  }

  return [...dayMap.values()]
    .map(buildDayRow)
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
        awakePercentageTotal: 0,
        lightPercentageTotal: 0,
        deepPercentageTotal: 0,
        remPercentageTotal: 0,
        percentageDayCount: 0,
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

    if ((row.totalStageCount ?? 0) > 0) {
      currentMonth.awakePercentageTotal += row.awakePercentage ?? 0;
      currentMonth.lightPercentageTotal += row.lightPercentage ?? 0;
      currentMonth.deepPercentageTotal += row.deepPercentage ?? 0;
      currentMonth.remPercentageTotal += row.remPercentage ?? 0;
      currentMonth.percentageDayCount += 1;
    }
  }

  return [...monthMap.values()]
    .map((month) => ({
      key: month.key,
      monthKey: month.monthKey,
      totalDurationMinutes: month.totalDurationMinutes,
      dayCount: month.dayCount,
      sessionCount: month.sessionCount,
      totalStageCount: month.totalStageCount,
      awakeMinutes: month.awakeMinutes,
      lightMinutes: month.lightMinutes,
      deepMinutes: month.deepMinutes,
      remMinutes: month.remMinutes,
      averageSleepMinutes: Math.round(month.totalDurationMinutes / month.dayCount),
      averageAwakeMinutes: Math.round(month.awakeMinutes / month.dayCount),
      averageLightMinutes: Math.round(month.lightMinutes / month.dayCount),
      averageDeepMinutes: Math.round(month.deepMinutes / month.dayCount),
      averageRemMinutes: Math.round(month.remMinutes / month.dayCount),
      awakePercentage:
        month.percentageDayCount > 0
          ? month.awakePercentageTotal / month.percentageDayCount
          : null,
      lightPercentage:
        month.percentageDayCount > 0
          ? month.lightPercentageTotal / month.percentageDayCount
          : null,
      deepPercentage:
        month.percentageDayCount > 0
          ? month.deepPercentageTotal / month.percentageDayCount
          : null,
      remPercentage:
        month.percentageDayCount > 0
          ? month.remPercentageTotal / month.percentageDayCount
          : null,
    }))
    .sort((a, b) => b.monthKey.localeCompare(a.monthKey));
}