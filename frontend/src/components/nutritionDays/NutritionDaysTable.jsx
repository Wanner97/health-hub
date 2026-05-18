import { formatDate } from '../../utils/date/dateFormatters';
import {
  formatCaloriesKcal,
  formatGrams,
  formatNumber,
} from '../../utils/number/numberFormatters';

function NutritionDaysTable({ rows }) {
  if (!rows?.length) {
    return (
      <section className="table-section">
        <h2>Ernährungsübersicht</h2>
        <p>Es wurden keine Ernährungsdaten gefunden.</p>
      </section>
    );
  }

  return (
    <section className="table-section">
      <h2>Ernährungsübersicht</h2>

      <table>
        <thead>
          <tr>
            <th>Datum</th>
            <th>Kalorien</th>
            <th>Records</th>
            <th>Fett</th>
            <th>Kohlenhydrate</th>
            <th>Protein</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((day) => (
            <tr key={day.date}>
              <td>{formatDate(day.date)}</td>
              <td>{formatCaloriesKcal(day.totalEnergyKcal)}</td>
              <td>{formatNumber(day.recordCount)}</td>
              <td>{formatGrams(day.totalFatGrams)}</td>
              <td>{formatGrams(day.totalCarbohydrateGrams)}</td>
              <td>{formatGrams(day.totalProteinGrams)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default NutritionDaysTable;