import {
  formatDate,
  formatDateUtcDateOnly,
} from '../date/dateFormatters';
import { formatDurationMinutes } from '../duration/durationFormatters';
import { formatNumber } from '../number/numberFormatters';

function formatPercent(value) {
  return `${formatNumber(Math.round(value ?? 0))}%`;
}

export function buildImportSubtitle(latestImport) {
  if (!latestImport) {
    return 'Noch keine Importdaten verfügbar.';
  }

  return `${formatDateUtcDateOnly(latestImport.importedAtUtc)} · Erhalten ${formatNumber(latestImport.receivedRecordCount)} · Neu ${formatNumber(latestImport.insertedRecordCount)} · Aktualisiert ${formatNumber(latestImport.updatedRecordCount)} · Unverändert ${formatNumber(latestImport.unchangedRecordCount)}`;
}

export function buildStepsTitle(latestActivityDay) {
  if (!latestActivityDay) {
    return 'Keine Schrittedaten';
  }

  return `${formatNumber(latestActivityDay.steps)} Schritte`;
}

export function buildStepsSubtitle(latestActivityDay) {
  if (!latestActivityDay) {
    return 'Kein aktueller Schrittedatensatz vorhanden.';
  }

  return `am ${formatDate(latestActivityDay.date)}`;
}

export function buildSleepTitle(latestSleepSession) {
  if (!latestSleepSession) {
    return 'Kein Schlafdatensatz';
  }

  return `${formatDurationMinutes(latestSleepSession.durationMinutes)} Schlaf`;
}

export function buildSleepSubtitle(latestSleepSession) {
  if (!latestSleepSession) {
    return 'Kein aktueller Schlafdatensatz vorhanden.';
  }

  const startDate = formatDateUtcDateOnly(latestSleepSession.startTimeUtc);
  const endDate = formatDateUtcDateOnly(latestSleepSession.endTimeUtc);

  return `vom ${startDate} auf den ${endDate}`;
}

export function buildHeartRateTitle(latestHeartRateDay) {
  if (!latestHeartRateDay) {
    return 'Keine Herzfrequenzdaten';
  }

  return `Ø ${formatNumber(latestHeartRateDay.avgBpm)} BPM`;
}

export function buildHeartRateSubtitle(latestHeartRateDay) {
  if (!latestHeartRateDay) {
    return 'Kein aktueller Herzfrequenzdatensatz vorhanden.';
  }

  return `am ${formatDate(latestHeartRateDay.date)} · Min ${formatNumber(latestHeartRateDay.minBpm)} · Max ${formatNumber(latestHeartRateDay.maxBpm)} · ${formatNumber(latestHeartRateDay.measurementCount)} Messungen`;
}

export function buildBloodOxygenTitle(latestBloodOxygenDay) {
  if (!latestBloodOxygenDay) {
    return 'Keine Blutsauerstoffdaten';
  }

  return `Ø ${formatPercent(latestBloodOxygenDay.avgPercent)} SpO₂`;
}

export function buildBloodOxygenSubtitle(latestBloodOxygenDay) {
  if (!latestBloodOxygenDay) {
    return 'Kein aktueller Blutsauerstoffdatensatz vorhanden.';
  }

  return `am ${formatDate(latestBloodOxygenDay.date)} · Min ${formatPercent(latestBloodOxygenDay.minPercent)} · Max ${formatPercent(latestBloodOxygenDay.maxPercent)} · ${formatNumber(latestBloodOxygenDay.measurementCount)} Messungen`;
}