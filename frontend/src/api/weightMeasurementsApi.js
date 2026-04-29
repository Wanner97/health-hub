import { apiRequest } from './apiClient';
import { buildMeasuredAtUtcRangeQuery } from '../utils/api/queryParams';

export async function getWeightMeasurements(filters = {}) {
  const query = buildMeasuredAtUtcRangeQuery(filters);
  return await apiRequest(`/api/WeightMeasurements${query}`);
}