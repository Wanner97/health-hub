import SummaryCard from '../SummaryCard';

function ActivityStatsSummary({
  rangeLabel,
  dayCount,
  averageSteps,
  averageDistance,
  totalSteps,
}) {
  return (
    <section className="summary-grid">
      <SummaryCard title="Zeitraum" value={rangeLabel} />
      <SummaryCard title="Tage im Datensatz" value={dayCount} />
      <SummaryCard title="Ø Schritte / Tag" value={averageSteps} />
      <SummaryCard title="Ø km / Tag" value={averageDistance} />
      <SummaryCard title="Schritte insgesamt" value={totalSteps} />
    </section>
  );
}

export default ActivityStatsSummary;