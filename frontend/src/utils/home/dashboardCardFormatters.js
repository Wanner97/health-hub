import {
  formatDate,
  formatDateUtcDateOnly,
} from '../date/dateFormatters';
import { formatDurationMinutes } from '../duration/durationFormatters';
import { formatNumber } from '../number/numberFormatters';

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