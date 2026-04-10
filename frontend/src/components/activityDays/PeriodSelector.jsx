import { PERIODS } from '../../utils/activityDays/periodUtils';

function PeriodSelector({ period, endDate, onPeriodChange, onEndDateChange }) {
  return (
    <section className="filter-bar">
      <div className="period-tabs">
        <button
          type="button"
          className={period === PERIODS.SEVEN_DAYS ? 'active' : ''}
          onClick={() => onPeriodChange(PERIODS.SEVEN_DAYS)}
        >
          7 Tage
        </button>

        <button
          type="button"
          className={period === PERIODS.THIRTY_ONE_DAYS ? 'active' : ''}
          onClick={() => onPeriodChange(PERIODS.THIRTY_ONE_DAYS)}
        >
          31 Tage
        </button>

        <button
          type="button"
          className={period === PERIODS.TWELVE_MONTHS ? 'active' : ''}
          onClick={() => onPeriodChange(PERIODS.TWELVE_MONTHS)}
        >
          12 Monate
        </button>
      </div>

      <div className="filter-group">
        <label htmlFor="endDate">Bis</label>
        <input
          id="endDate"
          type="date"
          value={endDate}
          onChange={(event) => onEndDateChange(event.target.value)}
        />
      </div>
    </section>
  );
}

export default PeriodSelector;