export function calculateTotalSteps(stepEntries) {
  if (!stepEntries?.length) {
    return 0;
  }

  return stepEntries.reduce((sum, entry) => sum + entry.count, 0);
}

export function calculateAverageSteps(stepEntries) {
  if (!stepEntries?.length) {
    return 0;
  }

  const total = calculateTotalSteps(stepEntries);
  return Math.round(total / stepEntries.length);
}

export function getHighestStepEntry(stepEntries) {
  if (!stepEntries?.length) {
    return null;
  }

  return [...stepEntries].sort((a, b) => b.count - a.count)[0];
}

export function getLowestStepEntry(stepEntries) {
  if (!stepEntries?.length) {
    return null;
  }

  return [...stepEntries].sort((a, b) => a.count - b.count)[0];
}

export function formatDateTime(value) {
  if (!value) {
    return '-';
  }

  return new Date(value).toLocaleString('de-CH');
}

export function formatNumber(value) {
  return new Intl.NumberFormat('de-CH').format(value ?? 0);
}

export function formatDate(value) {
  if (!value) {
    return '-';
  }

  const [year, month, day] = value.split('-');
  return `${day}.${month}.${year}`;
}