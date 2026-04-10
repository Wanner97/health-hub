import SummaryCard from '../SummaryCard';

function ActivityStatsSummary({
  periodLabel,
  dayCount,
  averageSteps,
  averageDistance,
  totalSteps,
}) {
  return (
    <section className="summary-grid">
      <SummaryCard title="Zeitraum" value={periodLabel} />
      <SummaryCard title="Tage im Datensatz" value={dayCount} />
      <SummaryCard title="Ø Schritte / Tag" value={averageSteps} />
      <SummaryCard title="Ø km / Tag" value={averageDistance} />
      <SummaryCard title="Schritte insgesamt" value={totalSteps} />
    </section>
  );
}

export default ActivityStatsSummary;