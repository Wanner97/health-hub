import { useEffect, useMemo, useState } from 'react';
import { getNutritionDays } from '../api/nutritionDaysApi';
import { PERIODS } from '../constants/periods';
import { VIEW_MODES } from '../constants/viewModes';
import { formatDateForInput } from '../utils/date/dateHelpers';
import { getRangeFromPeriod } from '../utils/periods/periodRangeUtils';
import { getNutritionChartData } from '../utils/nutritionDays/chartData';
import {
  sortNutritionDaysByDate,
  sortNutritionDaysByDateDesc,
} from '../utils/nutritionDays/transformers';

export function useNutritionDaysDashboard() {
  const [period, setPeriod] = useState(PERIODS.SEVEN_DAYS);
  const [endDate, setEndDate] = useState(formatDateForInput(new Date()));
  const [viewMode, setViewMode] = useState(VIEW_MODES.STATS);
  const [nutritionDays, setNutritionDays] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  const selectedRange = useMemo(() => {
    return getRangeFromPeriod(period, endDate);
  }, [period, endDate]);

  useEffect(() => {
    let isCancelled = false;

    async function loadNutritionDays() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getNutritionDays(selectedRange);
        const sorted = sortNutritionDaysByDate(data ?? []);

        if (!isCancelled) {
          setNutritionDays(sorted);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Ernährungstage:', error);

        if (!isCancelled) {
          setErrorMessage('Die Ernährungsdaten konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadNutritionDays();

    return () => {
      isCancelled = true;
    };
  }, [selectedRange]);

  const displayRows = useMemo(() => {
    return sortNutritionDaysByDateDesc(nutritionDays);
  }, [nutritionDays]);

  const chartData = useMemo(() => {
    return getNutritionChartData(period, nutritionDays, selectedRange);
  }, [period, nutritionDays, selectedRange]);

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
    nutritionDays,
    displayRows,
    chartData,
  };
}