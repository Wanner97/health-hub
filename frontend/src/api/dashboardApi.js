import { apiRequest } from './apiClient';

export async function getHomepageDashboard() {
  return await apiRequest('/api/dashboard/homepage');
}