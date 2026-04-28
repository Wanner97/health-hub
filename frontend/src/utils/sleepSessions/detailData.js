export function getDailySegments(item) {
  if (!item) {
    return [];
  }

  const totalKnownMinutes =
    (item.awakeMinutes ?? 0) +
    (item.remMinutes ?? 0) +
    (item.lightMinutes ?? 0) +
    (item.deepMinutes ?? 0);

  const unknownMinutes = Math.max(
    0,
    (item.totalDurationMinutes ?? 0) - totalKnownMinutes
  );

  return [
    {
      key: 'awake',
      minutes: item.awakeMinutes ?? 0,
      className: 'sleep-stage-preview-segment--awake',
    },
    {
      key: 'rem',
      minutes: item.remMinutes ?? 0,
      className: 'sleep-stage-preview-segment--rem',
    },
    {
      key: 'light',
      minutes: item.lightMinutes ?? 0,
      className: 'sleep-stage-preview-segment--light',
    },
    {
      key: 'deep',
      minutes: item.deepMinutes ?? 0,
      className: 'sleep-stage-preview-segment--deep',
    },
    {
      key: 'unknown',
      minutes: unknownMinutes,
      className: 'sleep-stage-preview-segment--unknown',
    },
  ].filter((segment) => segment.minutes > 0);
}

export function getPhaseSummaryItems(item, isMonthly) {
  return [
    {
      key: 'awake',
      label: 'Wach',
      minutes: isMonthly
        ? item.averageAwakeMinutes ?? 0
        : item.awakeMinutes ?? 0,
      percentage: item.awakePercentage,
      valueClassName: 'sleep-phase-stat-value--awake',
    },
    {
      key: 'rem',
      label: 'REM',
      minutes: isMonthly
        ? item.averageRemMinutes ?? 0
        : item.remMinutes ?? 0,
      percentage: item.remPercentage,
      valueClassName: 'sleep-phase-stat-value--rem',
    },
    {
      key: 'light',
      label: 'Leicht',
      minutes: isMonthly
        ? item.averageLightMinutes ?? 0
        : item.lightMinutes ?? 0,
      percentage: item.lightPercentage,
      valueClassName: 'sleep-phase-stat-value--light',
    },
    {
      key: 'deep',
      label: 'Tief',
      minutes: isMonthly
        ? item.averageDeepMinutes ?? 0
        : item.deepMinutes ?? 0,
      percentage: item.deepPercentage,
      valueClassName: 'sleep-phase-stat-value--deep',
    },
  ];
}