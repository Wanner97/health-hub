import { PERIODS } from '../../constants/periods';
import {
  formatGrams,
  formatPercent,
} from '../../utils/number/numberFormatters';

function getMacroRows(item, period) {
  const isMonthly = period === PERIODS.TWELVE_MONTHS;

  return [
    {
      key: 'carbs',
      label: 'Kohlenhydrate',
      shortLabel: 'KH',
      className: 'nutrition-macro-dot--carbs',
      percentClassName: 'nutrition-macro-percent--carbs',
      amount: isMonthly
        ? item.averageCarbohydrateGrams ?? 0
        : item.totalCarbohydrateGrams ?? 0,
      percent: item.carbohydratePercent ?? 0,
      colorVariable: 'var(--color-nutrition-carbs)',
    },
    {
      key: 'fat',
      label: 'Fett',
      shortLabel: 'Fett',
      className: 'nutrition-macro-dot--fat',
      percentClassName: 'nutrition-macro-percent--fat',
      amount: isMonthly
        ? item.averageFatGrams ?? 0
        : item.totalFatGrams ?? 0,
      percent: item.fatPercent ?? 0,
      colorVariable: 'var(--color-nutrition-fat)',
    },
    {
      key: 'protein',
      label: 'Protein',
      shortLabel: 'Protein',
      className: 'nutrition-macro-dot--protein',
      percentClassName: 'nutrition-macro-percent--protein',
      amount: isMonthly
        ? item.averageProteinGrams ?? 0
        : item.totalProteinGrams ?? 0,
      percent: item.proteinPercent ?? 0,
      colorVariable: 'var(--color-nutrition-protein)',
    },
  ];
}

function buildPieBackground(rows) {
  const totalPercent = rows.reduce((sum, row) => sum + row.percent, 0);

  if (totalPercent <= 0) {
    return 'var(--color-control)';
  }

  let currentStart = 0;

  const segments = rows.map((row) => {
    const normalizedPercent = (row.percent / totalPercent) * 100;
    const segmentStart = currentStart;
    const segmentEnd = currentStart + normalizedPercent;

    currentStart = segmentEnd;

    return `${row.colorVariable} ${segmentStart}% ${segmentEnd}%`;
  });

  return `conic-gradient(${segments.join(', ')})`;
}

function NutritionMacroBreakdown({ item, period }) {
  if (!item) {
    return null;
  }

  const rows = getMacroRows(item, period);
  const pieBackground = buildPieBackground(rows);

  return (
    <div className="nutrition-macro-breakdown">
      <div className="nutrition-macro-header">
        <h3>Makronährstoffe</h3>
        {period === PERIODS.TWELVE_MONTHS && (
          <span>Durchschnitt pro Tag</span>
        )}
      </div>

      <div className="nutrition-macro-content">
        <div
          className="nutrition-macro-pie"
          style={{ background: pieBackground }}
          aria-hidden="true"
        />

        <div className="nutrition-macro-list">
          {rows.map((row) => (
            <div key={row.key} className="nutrition-macro-row">
              <span
                className={`nutrition-macro-dot ${row.className}`}
                aria-hidden="true"
              />

              <div className="nutrition-macro-label-group">
                <span className="nutrition-macro-label">{row.shortLabel}</span>
                <span className="nutrition-macro-sub-label">{row.label}</span>
              </div>

              <div className="nutrition-macro-value-group">
                <span className={`nutrition-macro-percent ${row.percentClassName}`}>
                  {formatPercent(row.percent)}
                </span>
                <span className="nutrition-macro-value">
                  {formatGrams(row.amount)}
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default NutritionMacroBreakdown;