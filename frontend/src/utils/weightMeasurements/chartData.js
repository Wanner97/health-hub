import { PERIODS } from '../../constants/periods';
import {
  formatShortDateLabel,
  formatShortMonth,
} from '../date/dateFormatters';
import { formatDateForInput } from '../date/dateHelpers';
import {
  buildRangeScale,
  getRangePosition,
} from '../charts/rangeScaleUtils';
import { buildMonthlyWeightRows } from './transformers';

function formatDayOfMonthLabel(dateString) {
  if (!dateString) {
    return '';
  }

  const [, , day] = dateString.split('-');
  return day ?? '';
}

function addOneDay(date) {
  const nextDate = new Date(date);
  nextDate.setDate(nextDate.getDate() + 1);
  return nextDate;
}

function buildWeightMeasurementMap(weightMeasurements) {
  const map = new Map();

  for (const measurement of weightMeasurements ?? []) {
    if (!measurement?.date) {
      continue;
    }

    const current = map.get(measurement.date);

    if (!current) {
      map.set(measurement.date, measurement);
      continue;
    }

    const currentMeasuredAt = current.measuredAtUtc ?? '';
    const nextMeasuredAt = measurement.measuredAtUtc ?? '';

    if (nextMeasuredAt.localeCompare(currentMeasuredAt) > 0) {
      map.set(measurement.date, measurement);
    }
  }

  return map;
}

export function buildDailyWeightChartData(weightMeasurements, selectedRange) {
  if (!selectedRange?.from || !selectedRange?.to) {
    return [];
  }

  const byDate = buildWeightMeasurementMap(weightMeasurements);
  const result = [];

  let currentDate = new Date(`${selectedRange.from}T00:00:00`);

  while (formatDateForInput(currentDate) <= selectedRange.to) {
    const currentKey = formatDateForInput(currentDate);
    const measurement = byDate.get(currentKey);

    result.push({
      key: currentKey,
      label: formatDayOfMonthLabel(currentKey),
      fullLabel: currentKey,
      date: currentKey,
      measuredAtUtc: measurement?.measuredAtUtc ?? null,
      weightKg: measurement?.weightKg ?? null,
      hasMeasurement: Boolean(measurement),
    });

    currentDate = addOneDay(currentDate);
  }

  return result;
}

export function buildUnboundedWeightChartData(weightMeasurements) {
  return [...(weightMeasurements ?? [])]
    .sort((a, b) => {
      const dateComparison = (a.date ?? '').localeCompare(b.date ?? '');

      if (dateComparison !== 0) {
        return dateComparison;
      }

      return (a.measuredAtUtc ?? '').localeCompare(b.measuredAtUtc ?? '');
    })
    .map((measurement) => ({
      key: `${measurement.date}-${measurement.measuredAtUtc ?? ''}`,
      label: formatShortDateLabel(measurement.date),
      fullLabel: measurement.date,
      date: measurement.date,
      measuredAtUtc: measurement.measuredAtUtc ?? null,
      weightKg: measurement.weightKg ?? null,
      hasMeasurement: true,
    }));
}

export function buildMonthlyWeightChartData(weightMeasurements) {
  return buildMonthlyWeightRows(weightMeasurements)
    .sort((a, b) => a.monthKey.localeCompare(b.monthKey))
    .map((month) => ({
      key: month.monthKey,
      label: formatShortMonth(month.monthKey),
      fullLabel: month.monthKey,
      date: month.monthKey,
      weightKg: month.averageWeightKg,
      minWeightKg: month.minWeightKg,
      maxWeightKg: month.maxWeightKg,
      measurementCount: month.measurementCount,
      dayCount: month.dayCount,
      hasMeasurement: month.measurementCount > 0,
    }));
}

export function buildWeightChartData({
  period,
  selectedRange,
  weightMeasurements,
  useUnboundedFallback,
}) {
  if (
    useUnboundedFallback &&
    period !== PERIODS.TWELVE_MONTHS
  ) {
    return buildUnboundedWeightChartData(weightMeasurements);
  }

  if (period === PERIODS.TWELVE_MONTHS) {
    return buildMonthlyWeightChartData(weightMeasurements);
  }

  return buildDailyWeightChartData(weightMeasurements, selectedRange);
}

export function getMeasuredWeightChartItems(data) {
  return (data ?? []).filter(
    (item) => item.hasMeasurement && item.weightKg != null
  );
}

export function buildWeightLineScale(data) {
  const measuredItems = getMeasuredWeightChartItems(data);

  if (!measuredItems.length) {
    return {
      chartMin: 0,
      chartMax: 1,
      values: [],
    };
  }

  const minValue = Math.min(...measuredItems.map((item) => item.weightKg));
  const maxValue = Math.max(...measuredItems.map((item) => item.weightKg));

  return buildRangeScale(minValue, maxValue, {
    padding: 1,
    minLimit: 0,
    fallbackStep: 1,
  });
}

export function buildWeightLinePoints(data, chartMin, chartMax) {
  if (!data?.length) {
    return [];
  }

  return data
    .map((item, index) => {
      if (!item.hasMeasurement || item.weightKg == null) {
        return null;
      }

      const x = ((index + 0.5) / data.length) * 100;

      const y = 100 - getRangePosition(item.weightKg, chartMin, chartMax);

      return {
        key: item.key,
        item,
        x,
        y,
      };
    })
    .filter(Boolean);
}

export function buildWeightLinePath(points) {
  if (!points?.length || points.length < 2) {
    return '';
  }

  return points
    .map((point, index) => {
      const command = index === 0 ? 'M' : 'L';
      return `${command} ${point.x} ${point.y}`;
    })
    .join(' ');
}

export function buildWeightLineChartModel(data) {
  const {
    chartMin,
    chartMax,
    values: guideValues,
  } = buildWeightLineScale(data);

  const points = buildWeightLinePoints(data, chartMin, chartMax);
  const linePath = buildWeightLinePath(points);

  return {
    chartMin,
    chartMax,
    guideValues,
    points,
    linePath,
    hasLine: points.length >= 2,
  };
}