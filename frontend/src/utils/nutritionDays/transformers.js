export function sortNutritionDaysByDate(nutritionDays) {
  if (!nutritionDays?.length) {
    return [];
  }

  return [...nutritionDays].sort((a, b) =>
    (a.date ?? '').localeCompare(b.date ?? '')
  );
}

export function sortNutritionDaysByDateDesc(nutritionDays) {
  if (!nutritionDays?.length) {
    return [];
  }

  return [...nutritionDays].sort((a, b) =>
    (b.date ?? '').localeCompare(a.date ?? '')
  );
}