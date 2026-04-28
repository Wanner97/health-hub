import { PERIODS } from '../../constants/periods';
import {
  formatDate,
  formatMonthDetailsLabel,
} from '../date/dateFormatters';

export function getRangeDetailsLabel(item, period) {
  if (!item) {
    return '';
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return formatMonthDetailsLabel(item.fullLabel);
  }

  return formatDate(item.fullLabel);
}

export function buildRangeDetailsMeta({
  period,
  averageValue,
  measurementCount,
  dayCount,
}) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return `Ø ${averageValue} · ${measurementCount} Messungen · ${dayCount} Tage`;
  }

  return `Ø ${averageValue} · ${measurementCount} Messungen`;
}