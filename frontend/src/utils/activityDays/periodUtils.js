import { PERIODS } from '../../constants/activityDays';

export function formatDateForInput(date) {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
}

export function parseDateString(value) {
  const [year, month, day] = value.split('-').map(Number);
  return new Date(year, month - 1, day);
}

export function addDays(date, days) {
  const copy = new Date(date);
  copy.setDate(copy.getDate() + days);
  return copy;
}

export function addMonths(date, months) {
  const copy = new Date(date);
  copy.setMonth(copy.getMonth() + months);
  return copy;
}

export function startOfMonth(date) {
  return new Date(date.getFullYear(), date.getMonth(), 1);
}

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