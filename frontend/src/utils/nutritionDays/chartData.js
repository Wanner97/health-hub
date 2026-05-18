import { PERIODS } from '../../constants/periods';
import { formatShortMonth } from '../date/dateFormatters';
import { formatDateForInput } from '../date/dateHelpers';

function addOneDay(date) {
  const nextDate = new Date(date);
  nextDate.setDate(nextDate.getDate() + 1);
  return nextDate;
}

function formatDayOfMonthLabel(dateString) {
  if (!dateString) {
    return '';
  }

  const [, , day] = dateString.split('-');
  return day ?? '';
}

function getMonthKey(dateString) {
  return dateString?.slice(0, 7) ?? '';
}

function buildNutritionDayMap(nutritionDays) {
  const map = new Map();

  for (const day of nutritionDays ?? []) {
    if (day?.date) {
      map.set(day.date, day);
    }
  }

  return map;
}

function calculateMacroPercentagesFromGrams({
  totalCarbohydrateGrams,
  totalFatGrams,
  totalProteinGrams,
}) {
  const carbohydrateCalories = (totalCarbohydrateGrams ?? 0) * 4;
  const fatCalories = (totalFatGrams ?? 0) * 9;
  const proteinCalories = (totalProteinGrams ?? 0) * 4;

  const totalMacroCalories =
    carbohydrateCalories + fatCalories + proteinCalories;

  if (totalMacroCalories <= 0) {
    return {
      carbohydratePercent: 0,
      fatPercent: 0,
      proteinPercent: 0,
    };
  }

  return {
    carbohydratePercent: (carbohydrateCalories / totalMacroCalories) * 100,
    fatPercent: (fatCalories / totalMacroCalories) * 100,
    proteinPercent: (proteinCalories / totalMacroCalories) * 100,
  };
}

export function buildDailyNutritionChartData(nutritionDays, selectedRange) {
  if (!selectedRange?.from || !selectedRange?.to) {
    return [];
  }

  const byDate = buildNutritionDayMap(nutritionDays);
  const result = [];

  let currentDate = new Date(`${selectedRange.from}T00:00:00`);

  while (formatDateForInput(currentDate) <= selectedRange.to) {
    const currentKey = formatDateForInput(currentDate);
    const day = byDate.get(currentKey);

    result.push({
      key: currentKey,
      label: formatDayOfMonthLabel(currentKey),
      fullLabel: currentKey,
      date: currentKey,
      hasData: Boolean(day),

      recordCount: day?.recordCount ?? 0,
      totalEnergyKcal: day?.totalEnergyKcal ?? 0,
      totalCarbohydrateGrams: day?.totalCarbohydrateGrams ?? 0,
      totalFatGrams: day?.totalFatGrams ?? 0,
      totalProteinGrams: day?.totalProteinGrams ?? 0,

      carbohydratePercent: day?.carbohydratePercent ?? 0,
      fatPercent: day?.fatPercent ?? 0,
      proteinPercent: day?.proteinPercent ?? 0,

      records: day?.records ?? [],
    });

    currentDate = addOneDay(currentDate);
  }

  return result;
}

export function buildMonthlyNutritionChartData(nutritionDays) {
  if (!nutritionDays?.length) {
    return [];
  }

  const monthMap = new Map();

  for (const day of nutritionDays) {
    const monthKey = getMonthKey(day.date);

    if (!monthKey) {
      continue;
    }

    if (!monthMap.has(monthKey)) {
      monthMap.set(monthKey, {
        monthKey,
        dayCount: 0,
        recordCount: 0,
        totalEnergyKcal: 0,
        totalCarbohydrateGrams: 0,
        totalFatGrams: 0,
        totalProteinGrams: 0,
        records: [],
      });
    }

    const currentMonth = monthMap.get(monthKey);

    currentMonth.dayCount += 1;
    currentMonth.recordCount += day.recordCount ?? 0;
    currentMonth.totalEnergyKcal += day.totalEnergyKcal ?? 0;
    currentMonth.totalCarbohydrateGrams += day.totalCarbohydrateGrams ?? 0;
    currentMonth.totalFatGrams += day.totalFatGrams ?? 0;
    currentMonth.totalProteinGrams += day.totalProteinGrams ?? 0;
    currentMonth.records.push(...(day.records ?? []));
  }

  return [...monthMap.values()]
    .sort((a, b) => a.monthKey.localeCompare(b.monthKey))
    .map((month) => {
      const averageEnergyKcal =
        month.dayCount > 0 ? month.totalEnergyKcal / month.dayCount : 0;

      const averageCarbohydrateGrams =
        month.dayCount > 0
          ? month.totalCarbohydrateGrams / month.dayCount
          : 0;

      const averageFatGrams =
        month.dayCount > 0 ? month.totalFatGrams / month.dayCount : 0;

      const averageProteinGrams =
        month.dayCount > 0 ? month.totalProteinGrams / month.dayCount : 0;

      const macroPercentages = calculateMacroPercentagesFromGrams({
        totalCarbohydrateGrams: month.totalCarbohydrateGrams,
        totalFatGrams: month.totalFatGrams,
        totalProteinGrams: month.totalProteinGrams,
      });

      return {
        key: month.monthKey,
        label: formatShortMonth(month.monthKey),
        fullLabel: month.monthKey,
        date: month.monthKey,
        hasData: month.dayCount > 0,

        dayCount: month.dayCount,
        recordCount: month.recordCount,

        totalEnergyKcal: month.totalEnergyKcal,
        totalCarbohydrateGrams: month.totalCarbohydrateGrams,
        totalFatGrams: month.totalFatGrams,
        totalProteinGrams: month.totalProteinGrams,

        averageEnergyKcal,
        averageCarbohydrateGrams,
        averageFatGrams,
        averageProteinGrams,

        carbohydratePercent: macroPercentages.carbohydratePercent,
        fatPercent: macroPercentages.fatPercent,
        proteinPercent: macroPercentages.proteinPercent,

        records: month.records,
      };
    });
}

export function getNutritionChartData(period, nutritionDays, selectedRange) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return buildMonthlyNutritionChartData(nutritionDays);
  }

  return buildDailyNutritionChartData(nutritionDays, selectedRange);
}