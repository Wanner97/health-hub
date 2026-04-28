import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import { formatSleepPercentage } from '../../utils/sleepSessions/formatters';

function SleepPhaseStatsGrid({ items }) {
  if (!items?.length) {
    return null;
  }

  return (
    <div className="sleep-phase-stats-grid">
      {items.map((phaseItem) => (
        <div key={phaseItem.key} className="sleep-phase-stat">
          <span className="sleep-phase-stat-label">
            {phaseItem.label} ({formatDurationMinutes(phaseItem.minutes)})
          </span>
          <span
            className={`sleep-phase-stat-value ${phaseItem.valueClassName}`}
          >
            {formatSleepPercentage(phaseItem.percentage)}
          </span>
        </div>
      ))}
    </div>
  );
}

export default SleepPhaseStatsGrid;