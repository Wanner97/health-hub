import { useEffect, useState } from 'react';
import { getHomepageDashboard } from '../api/dashboardApi';

export function useHomepageDashboard() {
  const [dashboard, setDashboard] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    let isCancelled = false;

    async function loadHomepageDashboard() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getHomepageDashboard();

        if (!isCancelled) {
          setDashboard(data);
        }
      } catch (error) {
        console.error('Fehler beim Laden des Homepage-Dashboards:', error);

        if (!isCancelled) {
          setErrorMessage('Die Dashboard-Daten konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadHomepageDashboard();

    return () => {
      isCancelled = true;
    };
  }, []);

  return {
    dashboard,
    isLoading,
    errorMessage,
  };
}