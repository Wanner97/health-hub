import { VIEW_MODES } from '../../constants/viewModes';

function ViewModeToggle({ viewMode, onViewModeChange }) {
  return (
    <section className="view-mode-toggle">
      <button
        type="button"
        className={viewMode === VIEW_MODES.STATS ? 'active' : ''}
        onClick={() => onViewModeChange(VIEW_MODES.STATS)}
      >
        Statistik
      </button>

      <button
        type="button"
        className={viewMode === VIEW_MODES.TABLE ? 'active' : ''}
        onClick={() => onViewModeChange(VIEW_MODES.TABLE)}
      >
        Tabelle
      </button>
    </section>
  );
}

export default ViewModeToggle;