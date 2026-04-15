export function formatDurationMinutes(value) {
  const totalMinutes = value ?? 0;
  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;

  return `${hours}h ${minutes}min`;
}