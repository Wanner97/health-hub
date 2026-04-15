import { PERIODS } from '../../constants/periods';
import {
  addDays,
  addMonths,
  formatDateForInput,
  parseDateString,
  startOfMonth,
} from '../date/dateHelpers';

export function getRangeFromPeriod(period, endDateString) {
  const endDate = parseDateString(endDateString);

  if (period === PERIODS.SEVEN_DAYS) {
    const fromDate = addDays(endDate, -6);

    return {
      from: formatDateForInput(fromDate),
      to: formatDateForInput(endDate),
    };
  }

  if (period === PERIODS.THIRTY_ONE_DAYS) {
    const fromDate = addDays(endDate, -30);

    return {
      from: formatDateForInput(fromDate),
      to: formatDateForInput(endDate),
    };
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    const currentMonthStart = startOfMonth(endDate);
    const fromDate = addMonths(currentMonthStart, -11);

    return {
      from: formatDateForInput(fromDate),
      to: formatDateForInput(endDate),
    };
  }

  throw new Error(`Unknown period: ${period}`);
}