const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function apiRequest(path, options = {}) {
  const url = `${API_BASE_URL}${path}`;

  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {}),
    },
    ...options,
  });

  if (!response.ok) {
    let errorMessage = `HTTP ${response.status}`;

    try {
      const errorBody = await response.json();
      if (errorBody?.message) {
        errorMessage = errorBody.message;
      }
    } catch {

    }

    throw new Error(errorMessage);
  }

  if (response.status === 204) {
    return null;
  }

  return await response.json();
}