export function calculateStageDurationMinutes(stage) {
  if (!stage?.startTimeUtc || !stage?.endTimeUtc) {
    return 0;
  }

  const start = new Date(`${stage.startTimeUtc}Z`);
  const end = new Date(`${stage.endTimeUtc}Z`);

  return Math.max(0, Math.round((end - start) / 60000));
}

export function calculatePercentage(value, total) {
  if (!total) {
    return null;
  }

  return (value / total) * 100;
}

export function calculateTotalSleepMinutes(rows) {
  if (!rows?.length) {
    return 0;
  }

  return rows.reduce(
    (sum, row) => sum + (row.totalDurationMinutes ?? 0),
    0
  );
}

export function calculateAverageSleepMinutes(rows) {
  if (!rows?.length) {
    return 0;
  }

  return Math.round(calculateTotalSleepMinutes(rows) / rows.length);
}

export function calculateTotalSessionCount(rows) {
  if (!rows?.length) {
    return 0;
  }

  return rows.reduce((sum, row) => sum + (row.sessionCount ?? 0), 0);
}