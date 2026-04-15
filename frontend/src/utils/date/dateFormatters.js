import { MONTH_NAMES, MONTH_SHORT_NAMES } from '../../constants/months';

export function formatDate(value) {
  if (!value) {
    return '-';
  }

  const [year, month, day] = value.split('-');
  return `${day}.${month}.${year}`;
}

export function formatShortMonth(monthKey) {
  if (!monthKey) {
    return '-';
  }

  const [, month] = monthKey.split('-');
  return MONTH_SHORT_NAMES[month] ?? month;
}

export function formatMonthLabel(monthKey) {
  if (!monthKey) {
    return '-';
  }

  const [year] = monthKey.split('-');
  return `${formatShortMonth(monthKey)} ${year}`;
}

export function formatMonthDetailsLabel(monthKey) {
  if (!monthKey) {
    return '-';
  }

  const [year, month] = monthKey.split('-');
  return `${MONTH_NAMES[month]} ${year}`;
}

export function formatShortMonthYear(value) {
  if (!value) {
    return '-';
  }

  const [year] = value.split('-');
  return `${formatShortMonth(value)} ${year}`;
}

export function formatDateTimeUtc(value) {
  if (!value) {
    return '-';
  }

  return new Date(`${value}Z`).toLocaleString('de-CH');
}

export function formatDateUtcDateOnly(value) {
  if (!value) {
    return '-';
  }

  return new Date(`${value}Z`).toLocaleDateString('de-CH');
}

export function formatTimeUtc(value) {
  if (!value) {
    return '-';
  }

  return new Date(`${value}Z`).toLocaleTimeString('de-CH', {
    hour: '2-digit',
    minute: '2-digit',
  });
}