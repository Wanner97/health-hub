import { PERIODS } from '../../constants/periods';
import { formatTimeUtc } from '../../utils/date/dateFormatters';
import {
  formatCaloriesKcal,
  formatGrams,
  formatNumber,
} from '../../utils/number/numberFormatters';

function NutritionMealGroups({ mealGroups, period }) {
  if (period === PERIODS.TWELVE_MONTHS) {
    return null;
  }

  const visibleGroups =
    mealGroups?.filter((group) => group.records?.length > 0) ?? [];

  if (!visibleGroups.length) {
    return null;
  }

  return (
    <div className="nutrition-meal-groups">
      <div className="nutrition-detail-section-header">
        <h3>Einträge nach Mahlzeit</h3>
      </div>

      <div className="nutrition-meal-group-list">
        {visibleGroups.map((group) => (
          <section key={group.key} className="nutrition-meal-group">
            <div className="nutrition-meal-group-header">
              <div>
                <h4>{group.label}</h4>
                <span>
                  {formatNumber(group.records.length)} Einträge
                </span>
              </div>

              <strong>{formatCaloriesKcal(group.totalEnergyKcal)}</strong>
            </div>

            <div className="nutrition-meal-record-list">
              {group.records.map((record, index) => (
                <article
                  key={`${record.healthConnectRecordId ?? record.startTimeUtc}-${index}`}
                  className="nutrition-meal-record"
                >
                  <div className="nutrition-meal-record-main">
                    <div>
                      <h5>{record.name || 'Unbenannter Eintrag'}</h5>
                      <span>{formatTimeUtc(record.startTimeUtc)}</span>
                    </div>

                    <strong>
                      {formatCaloriesKcal(record.totalEnergyKcal)}
                    </strong>
                  </div>

                  <div className="nutrition-meal-record-macros">
                    <span>KH {formatGrams(record.totalCarbohydrateGrams)}</span>
                    <span>Fett {formatGrams(record.totalFatGrams)}</span>
                    <span>Protein {formatGrams(record.totalProteinGrams)}</span>
                  </div>
                </article>
              ))}
            </div>
          </section>
        ))}
      </div>
    </div>
  );
}

export default NutritionMealGroups;