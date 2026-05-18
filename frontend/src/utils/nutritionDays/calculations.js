export function calculateNutritionDayCount(nutritionDays) {
  return nutritionDays?.length ?? 0;
}

export function calculateTotalNutritionRecords(nutritionDays) {
  if (!nutritionDays?.length) {
    return 0;
  }

  return nutritionDays.reduce(
    (sum, day) => sum + (day.recordCount ?? 0),
    0
  );
}

export function calculateTotalEnergyKcal(nutritionDays) {
  if (!nutritionDays?.length) {
    return 0;
  }

  return nutritionDays.reduce(
    (sum, day) => sum + (day.totalEnergyKcal ?? 0),
    0
  );
}

export function calculateAverageEnergyKcalPerDay(nutritionDays) {
  if (!nutritionDays?.length) {
    return 0;
  }

  return calculateTotalEnergyKcal(nutritionDays) / nutritionDays.length;
}