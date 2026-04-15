import PeriodSelector from '../PeriodSelector';
import SleepSessionsSummary from './SleepSessionsSummary';
import SleepSessionsTable from './SleepSessionsTable';
import { useSleepSessions } from '../../hooks/useSleepSessions';
import { formatRangeLabel } from '../../utils/periods/periodFormatters';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import { formatNumber } from '../../utils/number/numberFormatters';

function SleepSessionsPage({ onBack }) {
  const {
    period,
    setPeriod,
    endDate,
    setEndDate,
    isLoading,
    errorMessage,
    selectedRange,
    dayCount,
    totalSleepMinutes,
    averageSleepMinutes,
    totalSessionCount,
    displayRows,
  } = useSleepSessions();

  return (
    <section className="sleep-page">
      <div className="page-header">
        <button type="button" className="back-button" onClick={onBack}>
          ← Zurück zum Dashboard
        </button>
      </div>

      <h1>Health Hub</h1>
      <p className="subtitle">Schlaf</p>

      <PeriodSelector
        period={period}
        endDate={endDate}
        onPeriodChange={setPeriod}
        onEndDateChange={setEndDate}
      />

      {isLoading && <p>Lade Schlafdaten...</p>}
      {errorMessage && <p className="error">{errorMessage}</p>}

      {!isLoading && !errorMessage && (
        <>
          <SleepSessionsSummary
            rangeLabel={formatRangeLabel(period, selectedRange.from, selectedRange.to)}
            dayCount={formatNumber(dayCount)}
            totalSessionCount={formatNumber(totalSessionCount)}
            averageSleep={formatDurationMinutes(averageSleepMinutes)}
            totalSleep={formatDurationMinutes(totalSleepMinutes)}
          />

          <SleepSessionsTable rows={displayRows} period={period} />
        </>
      )}
    </section>
  );
}

export default SleepSessionsPage;