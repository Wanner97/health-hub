import { PERIODS } from '../../constants/periods';
import { MONTH_SHORT_NAMES } from '../../constants/months';
import { formatShortMonthYear } from '../date/dateFormatters';

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