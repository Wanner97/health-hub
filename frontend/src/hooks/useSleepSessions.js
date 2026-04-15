import { useEffect, useMemo, useState } from 'react';
import { PERIODS } from '../constants/periods';
import { getSleepSessions } from '../api/sleepSessionsApi';
import { formatDateForInput } from '../utils/date/dateHelpers';
import { getRangeFromPeriod } from '../utils/periods/periodRangeUtils';
import {
  calculateAverageSleepMinutes,
  calculateTotalSessionCount,
  calculateTotalSleepMinutes,
} from '../utils/sleepSessions/calculations';
import {
  buildMonthlySleepRows,
  buildSleepDayRows,
} from '../utils/sleepSessions/transformers';

export function useSleepSessions() {
  const [period, setPeriod] = useState(PERIODS.SEVEN_DAYS);
  const [endDate, setEndDate] = useState(formatDateForInput(new Date()));
  const [sessions, setSessions] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  const selectedRange = useMemo(() => {
    return getRangeFromPeriod(period, endDate);
  }, [period, endDate]);

  useEffect(() => {
    let isCancelled = false;

    async function loadSleepSessions() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getSleepSessions(selectedRange);

        if (!isCancelled) {
          setSessions(data ?? []);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Sleep Sessions:', error);

        if (!isCancelled) {
          setErrorMessage('Die Schlafdaten konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadSleepSessions();

    return () => {
      isCancelled = true;
    };
  }, [selectedRange]);

  const sleepDayRows = useMemo(() => {
    return buildSleepDayRows(sessions);
  }, [sessions]);

  const displayRows = useMemo(() => {
    if (period === PERIODS.TWELVE_MONTHS) {
      return buildMonthlySleepRows(sleepDayRows);
    }

    return sleepDayRows;
  }, [period, sleepDayRows]);

  const dayCount = sleepDayRows.length;

  const totalSleepMinutes = useMemo(() => {
    return calculateTotalSleepMinutes(sleepDayRows);
  }, [sleepDayRows]);

  const averageSleepMinutes = useMemo(() => {
    return calculateAverageSleepMinutes(sleepDayRows);
  }, [sleepDayRows]);

  const totalSessionCount = useMemo(() => {
    return calculateTotalSessionCount(sleepDayRows);
  }, [sleepDayRows]);

  return {
    period,
    setPeriod,
    endDate,
    setEndDate,
    isLoading,
    errorMessage,
    selectedRange,
    dayCount,
    totalSleepMinutes,
    averageSleepMinutes,
    totalSessionCount,
    displayRows,
  };
}