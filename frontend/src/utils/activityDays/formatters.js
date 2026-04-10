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

export function formatDate(value) {
  if (!value) {
    return '-';
  }

  const [year, month, day] = value.split('-');
  return `${day}.${month}.${year}`;
}

export function formatDateTimeUtc(value) {
  if (!value) {
    return '-';
  }

  return new Date(`${value}Z`).toLocaleString('de-CH');
}

export function formatMonthLabel(monthKey) {
  if (!monthKey) {
    return '-';
  }

  const [year, month] = monthKey.split('-');
  return `${month}.${year}`;
}