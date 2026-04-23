import { apiRequest } from './apiClient';
import { buildDateRangeQuery } from '../utils/api/queryParams';

export async function getHeartRateDays(filters = {}) {
  const query = buildDateRangeQuery(filters, ['includeHourlyRecords']);
  return await apiRequest(`/api/heartratedays${query}`);
}