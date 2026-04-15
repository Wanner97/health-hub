import SummaryCard from '../SummaryCard';

function SleepSessionsSummary({
  rangeLabel,
  dayCount,
  totalSessionCount,
  averageSleep,
  totalSleep,
}) {
  return (
    <section className="summary-grid">
      <SummaryCard title="Zeitraum" value={rangeLabel} />
      <SummaryCard title="Tage im Datensatz" value={dayCount} />
      <SummaryCard title="Sleep Sessions" value={totalSessionCount} />
      <SummaryCard title="Ø Schlaf / Tag" value={averageSleep} />
      <SummaryCard title="Schlaf insgesamt" value={totalSleep} />
    </section>
  );
}

export default SleepSessionsSummary;