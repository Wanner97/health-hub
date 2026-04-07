function SummaryCard({ title, value }) {
  return (
    <article className="card">
      <h2>{title}</h2>
      <p>{value}</p>
    </article>
  );
}

export default SummaryCard;