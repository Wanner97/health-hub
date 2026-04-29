import { useEffect, useMemo, useState } from 'react';
import { getActivityDays } from '../api/activityDaysApi';
import { getChartData } from '../utils/activityDays/chartData';
import { PERIODS } from '../constants/periods';
import { VIEW_MODES } from '../constants/viewModes';
import {
  calculateAverageCaloriesBurnedKcal,
  calculateAverageDistanceMeters,
  calculateAverageSteps,
  calculateTotalCaloriesBurnedKcal,
  calculateTotalSteps,
} from '../utils/activityDays/calculations';
import { getRangeFromPeriod } from '../utils/periods/periodRangeUtils';
import { formatDateForInput } from '../utils/date/dateHelpers';
import {
  buildMonthlyAverageRows,
  fillMissingDays,
  sortActivityDaysByDate,
  sortActivityDaysByDateDesc,
} from '../utils/activityDays/transformers';

export function useActivityDaysDashboard() {
  const [period, setPeriod] = useState(PERIODS.SEVEN_DAYS);
  const [endDate, setEndDate] = useState(formatDateForInput(new Date()));
  const [viewMode, setViewMode] = useState(VIEW_MODES.STATS);
  const [activityDays, setActivityDays] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');
  const totalCaloriesBurned = calculateTotalCaloriesBurnedKcal(activityDays);
  const averageCaloriesBurned = calculateAverageCaloriesBurnedKcal(activityDays);

  const selectedRange = useMemo(() => {
    return getRangeFromPeriod(period, endDate);
  }, [period, endDate]);

  useEffect(() => {
    let isCancelled = false;

    async function loadActivityDays() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getActivityDays(selectedRange);
        const sorted = sortActivityDaysByDate(data);
        const completed = fillMissingDays(
          sorted,
          selectedRange.from,
          selectedRange.to
        );

        if (!isCancelled) {
          setActivityDays(completed);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Activity Days:', error);

        if (!isCancelled) {
          setErrorMessage('Die Activity-Daten konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadActivityDays();

    return () => {
      isCancelled = true;
    };
  }, [selectedRange]);

  const dayCount = activityDays.length;

  const totalSteps = useMemo(() => {
    return calculateTotalSteps(activityDays);
  }, [activityDays]);

  const averageSteps = useMemo(() => {
    return calculateAverageSteps(activityDays);
  }, [activityDays]);

  const averageDistance = useMemo(() => {
    return calculateAverageDistanceMeters(activityDays);
  }, [activityDays]);

  const displayRows = useMemo(() => {
    if (period === PERIODS.TWELVE_MONTHS) {
      return buildMonthlyAverageRows(activityDays);
    }

    return sortActivityDaysByDateDesc(activityDays);
  }, [period, activityDays]);

  const chartData = useMemo(() => {
    return getChartData(period, activityDays);
  }, [period, activityDays]);

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
    dayCount,
    totalSteps,
    totalCaloriesBurned,
    averageSteps,
    averageDistance,
    averageCaloriesBurned,
    displayRows,
    chartData,
  };
}