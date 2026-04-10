function ViewModeToggle({ viewMode, onViewModeChange }) {
  return (
    <section className="view-mode-toggle">
      <button
        type="button"
        className={viewMode === 'stats' ? 'active' : ''}
        onClick={() => onViewModeChange('stats')}
      >
        Statistik
      </button>

      <button
        type="button"
        className={viewMode === 'table' ? 'active' : ''}
        onClick={() => onViewModeChange('table')}
      >
        Tabelle
      </button>
    </section>
  );
}

export default ViewModeToggle;