export function formatNumber(value) {
  return new Intl.NumberFormat('de-CH').format(value ?? 0);
}

export function formatKilometersFromMeters(value) {
  const kilometers = (value ?? 0) / 1000;

  return new Intl.NumberFormat('de-CH', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(kilometers);
}