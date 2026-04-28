import { formatDate } from '../date/dateFormatters';

export function formatSleepDateRange(startKey, endKey) {
  if (!startKey || !endKey) {
    return '';
  }

  return `${formatDate(startKey)} – ${formatDate(endKey)}`;
}

export function formatSleepPercentage(value) {
  if (value == null) {
    return '-';
  }

  return `${Math.round(value)}%`;
}