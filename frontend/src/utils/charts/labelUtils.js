import { PERIODS } from '../../constants/periods';

export function shouldShowChartLabel(period, index, dataLength) {
  if (period === PERIODS.SEVEN_DAYS) {
    return true;
  }

  if (period === PERIODS.THIRTY_ONE_DAYS) {
    return index % 3 === 0 || index === dataLength - 1;
  }

  return true;
}