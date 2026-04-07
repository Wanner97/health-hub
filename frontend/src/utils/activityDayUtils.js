export function sortActivityDaysByDate(activityDays) {
  if (!activityDays?.length) {
    return [];
  }

  return [...activityDays].sort((a, b) => a.date.localeCompare(b.date));
}

export function formatDate(value) {
  if (!value) {
    return '-';
  }

  const [year, month, day] = value.split('-');
  return `${day}.${month}.${year}`;
}

export function formatDateTimeUtc(value) {
  if (!value) {
    return '-';
  }

  return new Date(`${value}Z`).toLocaleString('de-CH');
}

export function formatNumber(value) {
  return new Intl.NumberFormat('de-CH').format(value ?? 0);
}

export function formatMeters(value) {
  return new Intl.NumberFormat('de-CH', {
    maximumFractionDigits: 0,
  }).format(value ?? 0);
}

export function calculateTotalSteps(activityDays) {
  if (!activityDays?.length) {
    return 0;
  }

  return activityDays.reduce((sum, day) => sum + (day.steps ?? 0), 0);
}

export function calculateAverageSteps(activityDays) {
  if (!activityDays?.length) {
    return 0;
  }

  return Math.round(calculateTotalSteps(activityDays) / activityDays.length);
}

export function calculateTotalDistanceMeters(activityDays) {
  if (!activityDays?.length) {
    return 0;
  }

  return activityDays.reduce((sum, day) => sum + (day.distanceMeters ?? 0), 0);
}

export function getHighestStepDay(activityDays) {
  if (!activityDays?.length) {
    return null;
  }

  return [...activityDays].sort((a, b) => b.steps - a.steps)[0];
}

export function getLowestStepDay(activityDays) {
  if (!activityDays?.length) {
    return null;
  }

  return [...activityDays].sort((a, b) => a.steps - b.steps)[0];
}

export function calculateLastNDaysAverage(activityDays, days = 7) {
  if (!activityDays?.length) {
    return 0;
  }

  const sorted = sortActivityDaysByDate(activityDays);
  const slice = sorted.slice(-days);

  if (!slice.length) {
    return 0;
  }

  const total = slice.reduce((sum, day) => sum + (day.steps ?? 0), 0);
  return Math.round(total / slice.length);
}

export function getDateRangeLabel(activityDays) {
  if (!activityDays?.length) {
    return '-';
  }

  const sorted = sortActivityDaysByDate(activityDays);
  const first = sorted[0];
  const last = sorted[sorted.length - 1];

  return `${formatDate(first.date)} – ${formatDate(last.date)}`;
}