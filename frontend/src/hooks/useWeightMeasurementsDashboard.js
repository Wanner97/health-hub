import { useEffect, useMemo, useState } from 'react';
import { getWeightMeasurements } from '../api/weightMeasurementsApi';
import { PERIODS } from '../constants/periods';
import { VIEW_MODES } from '../constants/viewModes';
import { formatDateForInput } from '../utils/date/dateHelpers';
import { getRangeFromPeriod } from '../utils/periods/periodRangeUtils';
import {
  buildMonthlyWeightRows,
  sortWeightMeasurementsByDate,
  sortWeightMeasurementsByDateDesc,
  takeLatestWeightMeasurements,
} from '../utils/weightMeasurements/transformers';
import { buildWeightChartData } from '../utils/weightMeasurements/chartData';
import {
  calculateAverageWeightKg,
  calculateMaxWeightKg,
  calculateMinWeightKg,
  calculateWeightMeasurementCount,
} from '../utils/weightMeasurements/calculations';

function buildMeasuredAtUtcRange(selectedRange) {
  return {
    fromMeasuredAtUtc: `${selectedRange.from}T00:00:00Z`,
    toMeasuredAtUtc: `${selectedRange.to}T23:59:59.999Z`,
  };
}

function canUseUnboundedFallback(period) {
  return (
    period === PERIODS.SEVEN_DAYS ||
    period === PERIODS.THIRTY_ONE_DAYS
  );
}

function getFallbackMeasurementLimit(period) {
  if (period === PERIODS.SEVEN_DAYS) {
    return 7;
  }

  if (period === PERIODS.THIRTY_ONE_DAYS) {
    return 31;
  }

  return 0;
}

export function useWeightMeasurementsDashboard() {
  const [period, setPeriod] = useState(PERIODS.SEVEN_DAYS);
  const [endDate, setEndDate] = useState(formatDateForInput(new Date()));
  const [viewMode, setViewMode] = useState(VIEW_MODES.STATS);
  const [weightMeasurements, setWeightMeasurements] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');
  const [usesUnboundedFallback, setUsesUnboundedFallback] = useState(false);

  const selectedRange = useMemo(() => {
    return getRangeFromPeriod(period, endDate);
  }, [period, endDate]);

  useEffect(() => {
    let isCancelled = false;

    async function loadWeightMeasurements() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const boundedData = await getWeightMeasurements(
          buildMeasuredAtUtcRange(selectedRange)
        );

        let nextWeightMeasurements = sortWeightMeasurementsByDate(boundedData ?? []);
        let nextUsesUnboundedFallback = false;

        if (
          canUseUnboundedFallback(period) &&
          nextWeightMeasurements.length < 2
        ) {
          const unboundedData = await getWeightMeasurements();

          nextWeightMeasurements = takeLatestWeightMeasurements(
            unboundedData ?? [],
            getFallbackMeasurementLimit(period)
          );

          nextUsesUnboundedFallback = true;
        }

        if (!isCancelled) {
          setWeightMeasurements(nextWeightMeasurements);
          setUsesUnboundedFallback(nextUsesUnboundedFallback);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Gewichtsdaten:', error);

        if (!isCancelled) {
          setErrorMessage('Die Gewichtsdaten konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadWeightMeasurements();

    return () => {
      isCancelled = true;
    };
  }, [period, selectedRange]);

  const displayRows = useMemo(() => {
    if (period === PERIODS.TWELVE_MONTHS) {
      return buildMonthlyWeightRows(weightMeasurements);
    }

    return sortWeightMeasurementsByDateDesc(weightMeasurements);
  }, [period, weightMeasurements]);

  const chartData = useMemo(() => {
    return buildWeightChartData({
      period,
      selectedRange,
      weightMeasurements,
      useUnboundedFallback: usesUnboundedFallback,
    });
  }, [period, selectedRange, weightMeasurements, usesUnboundedFallback]);

  const summaryData = useMemo(() => {
    return {
      measurementCount: calculateWeightMeasurementCount(weightMeasurements),
      averageWeightKg: calculateAverageWeightKg(weightMeasurements),
      minWeightKg: calculateMinWeightKg(weightMeasurements),
      maxWeightKg: calculateMaxWeightKg(weightMeasurements),
    };
  }, [weightMeasurements]);

  return {
    period,
    setPeriod,
    endDate,
    setEndDate,
    viewMode,
    setViewMode,
    isLoading,
    errorMessage,
    selectedRange,
    weightMeasurements,
    displayRows,
    chartData,
    summaryData,
    usesUnboundedFallback
  };
}