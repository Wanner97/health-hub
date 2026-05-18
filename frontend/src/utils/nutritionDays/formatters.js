import {
  formatCaloriesKcal,
  formatNumber,
} from '../number/numberFormatters';

const NUTRIENT_LABELS = {
  energyKcal: 'Kalorien',
  totalCarbohydrateGrams: 'Kohlenhydrate',
  sugarGrams: 'Zucker',
  dietaryFiberGrams: 'Ballaststoffe',
  totalFatGrams: 'Fett',
  saturatedFatGrams: 'Gesättigte Fettsäuren',
  unsaturatedFatGrams: 'Ungesättigte Fettsäuren',
  monounsaturatedFatGrams: 'Einfach ungesättigte Fettsäuren',
  polyunsaturatedFatGrams: 'Mehrfach ungesättigte Fettsäuren',
  transFatGrams: 'Transfette',
  proteinGrams: 'Protein',
  sodiumGrams: 'Natrium',
  potassiumGrams: 'Kalium',
  calciumGrams: 'Kalzium',
  magnesiumGrams: 'Magnesium',
  phosphorusGrams: 'Phosphor',
  ironGrams: 'Eisen',
  zincGrams: 'Zink',
  copperGrams: 'Kupfer',
  manganeseGrams: 'Mangan',
  seleniumGrams: 'Selen',
  cholesterolGrams: 'Cholesterin',
  caffeineGrams: 'Koffein',
  biotinGrams: 'Biotin',
  folateGrams: 'Folat',
  niacinGrams: 'Niacin',
  pantothenicAcidGrams: 'Pantothensäure',
  riboflavinGrams: 'Riboflavin',
  vitaminAGrams: 'Vitamin A',
  vitaminB6Grams: 'Vitamin B6',
  vitaminB12Grams: 'Vitamin B12',
  vitaminCGrams: 'Vitamin C',
  vitaminDGrams: 'Vitamin D',
  vitaminEGrams: 'Vitamin E',
  vitaminKGrams: 'Vitamin K',
};

function formatDecimal(value, maximumFractionDigits = 2) {
  return new Intl.NumberFormat('de-CH', {
    minimumFractionDigits: 0,
    maximumFractionDigits,
  }).format(value ?? 0);
}

export function formatNutrientLabel(nutrientKey) {
  return NUTRIENT_LABELS[nutrientKey] ?? nutrientKey;
}

export function formatNutrientAmount(amount, unit, nutrientKey) {
  if (nutrientKey === 'energyKcal' || unit === 'kcal') {
    return formatCaloriesKcal(amount);
  }

  if (unit === 'g') {
    const absoluteAmount = Math.abs(amount ?? 0);

    if (absoluteAmount === 0) {
      return '0 g';
    }

    if (absoluteAmount >= 1) {
      return `${formatDecimal(amount, 1)} g`;
    }

    if (absoluteAmount >= 0.001) {
      return `${formatDecimal((amount ?? 0) * 1000, 2)} mg`;
    }

    return `${formatDecimal((amount ?? 0) * 1000000, 2)} µg`;
  }

  if (!unit) {
    return formatNumber(amount);
  }

  return `${formatDecimal(amount, 2)} ${unit}`;
}