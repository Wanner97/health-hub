import { useEffect, useState } from 'react';
import { getImportBatches } from '../api/importBatchesApi';

export function useImportBatches() {
  const [batches, setBatches] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    let isCancelled = false;

    async function loadImportBatches() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getImportBatches();

        if (!isCancelled) {
          setBatches(data ?? []);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Import-Batches:', error);

        if (!isCancelled) {
          setErrorMessage('Die Import-Batches konnten nicht geladen werden.');
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    }

    loadImportBatches();

    return () => {
      isCancelled = true;
    };
  }, []);

  return {
    batches,
    isLoading,
    errorMessage,
  };
}