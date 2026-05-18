import SummaryCard from '../SummaryCard';

function NutritionDaysStatsSummary({
  rangeLabel,
  dayCount,
  averageEnergyKcal,
  totalRecordCount,
  totalEnergyKcal,
}) {
  return (
    <section className="summary-grid summary-grid--nutrition">
      <SummaryCard title="Zeitraum" value={rangeLabel} />
      <SummaryCard title="Tage im Datensatz" value={dayCount} />
      <SummaryCard title="Ø kcal / Tag" value={averageEnergyKcal} />
      <SummaryCard title="Records insgesamt" value={totalRecordCount} />
      <SummaryCard title="Kalorien insgesamt" value={totalEnergyKcal} />
    </section>
  );
}

export default NutritionDaysStatsSummary;