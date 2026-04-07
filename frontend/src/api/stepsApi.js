const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function getLatestSteps() {
  const response = await fetch(`${API_BASE_URL}/api/steps/latest`);

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}`);
  }

  return await response.json();
}