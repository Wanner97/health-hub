const MEAL_GROUPS = [
  {
    key: 'breakfast',
    label: 'Frühstück',
  },
  {
    key: 'lunch',
    label: 'Mittagessen',
  },
  {
    key: 'dinner',
    label: 'Abendessen',
  },
  {
    key: 'snack',
    label: 'Snack',
  },
];

function getLocalMinutesFromUtc(value) {
  if (!value) {
    return null;
  }

  const date = new Date(`${value}Z`);

  if (Number.isNaN(date.getTime())) {
    return null;
  }

  return date.getHours() * 60 + date.getMinutes();
}

function getMealGroupKey(record) {
  const minutes = getLocalMinutesFromUtc(record.startTimeUtc);

  if (minutes == null) {
    return 'snack';
  }

  const breakfastStart = 0;
  const breakfastEnd = 11 * 60;

  const lunchStart = 11 * 60 + 1;
  const lunchEnd = 13 * 60;

  const dinnerStart = 17 * 60 + 30;
  const dinnerEnd = 22 * 60;

  if (minutes >= breakfastStart && minutes <= breakfastEnd) {
    return 'breakfast';
  }

  if (minutes >= lunchStart && minutes <= lunchEnd) {
    return 'lunch';
  }

  if (minutes >= dinnerStart && minutes <= dinnerEnd) {
    return 'dinner';
  }

  return 'snack';
}

function sortRecordsByStartTime(records) {
  return [...(records ?? [])].sort((a, b) =>
    (a.startTimeUtc ?? '').localeCompare(b.startTimeUtc ?? '')
  );
}

export function buildNutritionMealGroups(records) {
  const groups = new Map(
    MEAL_GROUPS.map((group) => [
      group.key,
      {
        ...group,
        totalEnergyKcal: 0,
        records: [],
      },
    ])
  );

  for (const record of sortRecordsByStartTime(records)) {
    const groupKey = getMealGroupKey(record);
    const group = groups.get(groupKey) ?? groups.get('snack');

    group.totalEnergyKcal += record.totalEnergyKcal ?? 0;
    group.records.push(record);
  }

  return MEAL_GROUPS.map((group) => groups.get(group.key));
}

export function buildNutritionNutrientTotals(records) {
  const nutrientMap = new Map();

  for (const record of records ?? []) {
    for (const nutrient of record.nutrients ?? []) {
      const nutrientKey = nutrient.nutrientKey ?? 'unknown';
      const unit = nutrient.unit ?? '';
      const mapKey = `${nutrientKey}|${unit}`;

      if (!nutrientMap.has(mapKey)) {
        nutrientMap.set(mapKey, {
          key: mapKey,
          nutrientKey,
          unit,
          amount: 0,
        });
      }

      const current = nutrientMap.get(mapKey);
      current.amount += nutrient.amount ?? 0;
    }
  }

  return [...nutrientMap.values()]
    .filter((nutrient) => Math.abs(nutrient.amount ?? 0) > 0)
    .sort((a, b) => a.nutrientKey.localeCompare(b.nutrientKey));
}

export function buildNutritionDetails(item) {
  if (!item?.hasData) {
    return {
      mealGroups: [],
      nutrientTotals: [],
    };
  }

  return {
    mealGroups: buildNutritionMealGroups(item.records),
    nutrientTotals: buildNutritionNutrientTotals(item.records),
  };
}