import { useEffect, useMemo, useState } from 'react';
import { getHeartRateDays } from '../api/heartRateDaysApi';
import { PERIODS } from '../constants/periods';
import { VIEW_MODES } from '../constants/viewModes';
import { formatDateForInput } from '../utils/date/dateHelpers';
import { getRangeFromPeriod } from '../utils/periods/periodRangeUtils';
import { getHeartRateChartData } from '../utils/heartRateDays/chartData';
import {
  buildMonthlyHeartRateRows,
  sortHeartRateDaysByDate,
  sortHeartRateDaysByDateDesc,
} from '../utils/heartRateDays/transformers';

export function useHeartRateDaysDashboard() {
  const [period, setPeriod] = useState(PERIODS.SEVEN_DAYS);
  const [endDate, setEndDate] = useState(formatDateForInput(new Date()));
  const [viewMode, setViewMode] = useState(VIEW_MODES.STATS);
  const [heartRateDays, setHeartRateDays] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  const selectedRange = useMemo(() => {
    return getRangeFromPeriod(period, endDate);
  }, [period, endDate]);

  const includeHourlyRecords = useMemo(() => {
    return period !== PERIODS.TWELVE_MONTHS;
  }, [period]);

  useEffect(() => {
    let isCancelled = false;

    async function loadHeartRateDays() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getHeartRateDays({
          ...selectedRange,
          includeHourlyRecords,
        });

        const sorted = sortHeartRateDaysByDate(data ?? []);

        if (!isCancelled) {
          setHeartRateDays(sorted);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Heart Rate Days:', error);

        if (!isCancelled) {
          setErrorMessage('Die Herzfrequenz-Daten konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadHeartRateDays();

    return () => {
      isCancelled = true;
    };
  }, [selectedRange, includeHourlyRecords]);

  const displayRows = useMemo(() => {
    if (period === PERIODS.TWELVE_MONTHS) {
      return buildMonthlyHeartRateRows(heartRateDays);
    }

    return sortHeartRateDaysByDateDesc(heartRateDays);
  }, [heartRateDays, period]);

  const chartData = useMemo(() => {
    return getHeartRateChartData(period, heartRateDays);
  }, [period, heartRateDays]);

  const dayCount = heartRateDays.length;

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
    includeHourlyRecords,
    heartRateDays,
    displayRows,
    chartData,
    dayCount,
  };
}