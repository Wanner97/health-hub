import { PERIODS } from './periodUtils';

const MONTH_NAMES = {
  '01': 'Januar',
  '02': 'Februar',
  '03': 'März',
  '04': 'April',
  '05': 'Mai',
  '06': 'Juni',
  '07': 'Juli',
  '08': 'August',
  '09': 'September',
  '10': 'Oktober',
  '11': 'November',
  '12': 'Dezember',
};

const MONTH_SHORT_NAMES = {
  '01': 'Jan',
  '02': 'Feb',
  '03': 'Mär',
  '04': 'Apr',
  '05': 'Mai',
  '06': 'Jun',
  '07': 'Jul',
  '08': 'Aug',
  '09': 'Sep',
  '10': 'Okt',
  '11': 'Nov',
  '12': 'Dez',
};

export function formatNumber(value) {
  return new Intl.NumberFormat('de-CH').format(value ?? 0);
}

export function formatKilometersFromMeters(value) {
  const kilometers = (value ?? 0) / 1000;

  return new Intl.NumberFormat('de-CH', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(kilometers);
}

export function formatDate(value) {
  if (!value) {
    return '-';
  }

  const [year, month, day] = value.split('-');
  return `${day}.${month}.${year}`;
}

export function formatLongDate(value) {
  if (!value) {
    return '-';
  }

  const [year, month, day] = value.split('-');
  return `${day}. ${MONTH_NAMES[month]} ${year}`;
}

export function formatMonthYear(value) {
  if (!value) {
    return '-';
  }

  const [year, month] = value.split('-');
  return `${MONTH_NAMES[month]} ${year}`;
}

export function formatShortMonthYear(value) {
  if (!value) {
    return '-';
  }

  const [year, month] = value.split('-');
  return `${MONTH_SHORT_NAMES[month]} ${year}`;
}

export function formatRangeLabel(period, from, to) {
  if (!from || !to) {
    return '-';
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return `${formatShortMonthYear(from)} – ${formatShortMonthYear(to)}`;
  }

  const [fromYear, fromMonth, fromDay] = from.split('-');
  const [toYear, toMonth, toDay] = to.split('-');

  const fromMonthShort = MONTH_SHORT_NAMES[fromMonth];
  const toMonthShort = MONTH_SHORT_NAMES[toMonth];

  if (fromYear === toYear) {
    return `${fromDay}. ${fromMonthShort} – ${toDay}. ${toMonthShort} ${toYear}`;
  }

  return `${fromDay}. ${fromMonthShort} ${fromYear} – ${toDay}. ${toMonthShort} ${toYear}`;
}

export function formatDateTimeUtc(value) {
  if (!value) {
    return '-';
  }

  return new Date(`${value}Z`).toLocaleString('de-CH');
}

export function formatMonthLabel(monthKey) {
  if (!monthKey) {
    return '-';
  }

  const [year, month] = monthKey.split('-');
  return `${month}.${year}`;
}