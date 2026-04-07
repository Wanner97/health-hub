function DateRangeFilter({ filters, onChange, onApply, onReset, disabled }) {
  return (
    <form className="filter-bar" onSubmit={onApply}>
      <div className="filter-group">
        <label htmlFor="from">Von</label>
        <input
          id="from"
          name="from"
          type="date"
          value={filters.from}
          onChange={onChange}
          disabled={disabled}
        />
      </div>

      <div className="filter-group">
        <label htmlFor="to">Bis</label>
        <input
          id="to"
          name="to"
          type="date"
          value={filters.to}
          onChange={onChange}
          disabled={disabled}
        />
      </div>

      <div className="filter-actions">
        <button type="submit" disabled={disabled}>
          Anwenden
        </button>

        <button type="button" onClick={onReset} disabled={disabled}>
          Zurücksetzen
        </button>
      </div>
    </form>
  );
}

export default DateRangeFilter;