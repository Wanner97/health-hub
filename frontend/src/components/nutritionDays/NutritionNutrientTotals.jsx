import { PERIODS } from '../../constants/periods';
import {
  formatNutrientAmount,
  formatNutrientLabel,
} from '../../utils/nutritionDays/formatters';

const NUTRIENT_DISPLAY_PRIORITY = {
  energyKcal: 0,
  totalEnergyKcal: 0,

  totalCarbohydrateGrams: 10,
  carbohydrateGrams: 10,

  totalFatGrams: 11,
  fatGrams: 11,

  proteinGrams: 12,
  totalProteinGrams: 12,
};

function getNutrientDisplayPriority(nutrientKey) {
  return NUTRIENT_DISPLAY_PRIORITY[nutrientKey] ?? 100;
}

function sortNutrientsForDisplay(nutrientTotals) {
  return [...(nutrientTotals ?? [])].sort((a, b) => {
    const priorityComparison =
      getNutrientDisplayPriority(a.nutrientKey) -
      getNutrientDisplayPriority(b.nutrientKey);

    if (priorityComparison !== 0) {
      return priorityComparison;
    }

    return formatNutrientLabel(a.nutrientKey).localeCompare(
      formatNutrientLabel(b.nutrientKey),
      'de-CH'
    );
  });
}

function NutritionNutrientTotals({ nutrientTotals, period }) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return null;
  }

  if (!nutrientTotals?.length) {
    return null;
  }

  const sortedNutrients = sortNutrientsForDisplay(nutrientTotals);

  return (
    <div className="nutrition-nutrient-totals">
      <div className="nutrition-detail-section-header">
        <h3>Nährstoffe gesamt</h3>
      </div>

      <div className="nutrition-nutrient-grid">
        {sortedNutrients.map((nutrient) => (
          <div key={nutrient.key} className="nutrition-nutrient-item">
            <span className="nutrition-nutrient-label">
              {formatNutrientLabel(nutrient.nutrientKey)}
            </span>

            <strong className="nutrition-nutrient-value">
              {formatNutrientAmount(
                nutrient.amount,
                nutrient.unit,
                nutrient.nutrientKey
              )}
            </strong>
          </div>
        ))}
      </div>
    </div>
  );
}

export default NutritionNutrientTotals;