import { useEffect, useMemo, useState } from 'react';
import { getBloodOxygenDays } from '../api/bloodOxygenDaysApi';
import { PERIODS } from '../constants/periods';
import { VIEW_MODES } from '../constants/viewModes';
import { formatDateForInput } from '../utils/date/dateHelpers';
import { getRangeFromPeriod } from '../utils/periods/periodRangeUtils';
import { getBloodOxygenChartData } from '../utils/bloodOxygenDays/chartData';
import {
  buildMonthlyBloodOxygenRows,
  sortBloodOxygenDaysByDate,
  sortBloodOxygenDaysByDateDesc,
} from '../utils/bloodOxygenDays/transformers';

export function useBloodOxygenDaysDashboard() {
  const [period, setPeriod] = useState(PERIODS.SEVEN_DAYS);
  const [endDate, setEndDate] = useState(formatDateForInput(new Date()));
  const [viewMode, setViewMode] = useState(VIEW_MODES.STATS);
  const [bloodOxygenDays, setBloodOxygenDays] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  const selectedRange = useMemo(() => {
    return getRangeFromPeriod(period, endDate);
  }, [period, endDate]);

  useEffect(() => {
    let isCancelled = false;

    async function loadBloodOxygenDays() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getBloodOxygenDays(selectedRange);
        const sorted = sortBloodOxygenDaysByDate(data ?? []);

        if (!isCancelled) {
          setBloodOxygenDays(sorted);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Blutsauerstoff-Datensätze:', error);

        if (!isCancelled) {
          setErrorMessage('Die Blutsauerstoff-Daten konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadBloodOxygenDays();

    return () => {
      isCancelled = true;
    };
  }, [selectedRange]);

  const displayRows = useMemo(() => {
    if (period === PERIODS.TWELVE_MONTHS) {
      return buildMonthlyBloodOxygenRows(bloodOxygenDays);
    }

    return sortBloodOxygenDaysByDateDesc(bloodOxygenDays);
  }, [bloodOxygenDays, period]);

  const chartData = useMemo(() => {
    return getBloodOxygenChartData(period, bloodOxygenDays);
  }, [period, bloodOxygenDays]);

  const dayCount = bloodOxygenDays.length;

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
    bloodOxygenDays,
    displayRows,
    chartData,
    dayCount,
  };
}