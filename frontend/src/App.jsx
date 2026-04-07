import { useEffect, useMemo, useState } from 'react';
import './App.css';
import { getLatestSteps } from './api/stepsApi';
import SummaryCard from './components/SummaryCard';
import StepsTable from './components/StepsTable';
import {
  calculateAverageSteps,
  calculateTotalSteps,
  formatDate,
  formatDateTime,
  formatNumber,
  getHighestStepEntry,
  getLowestStepEntry,
} from './utils/stepsUtils';

function App() {
  const [stepsData, setStepsData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    async function fetchLatestSteps() {
      try {
        setIsLoading(true);
        setErrorMessage('');

        const data = await getLatestSteps();
        setStepsData(data);
      } catch (error) {
        console.error('Fehler beim Laden der Steps:', error);
        setErrorMessage('Die Steps-Daten konnten nicht geladen werden.');
      } finally {
        setIsLoading(false);
      }
    }

    fetchLatestSteps();
  }, []);

  const totalSteps = useMemo(() => {
    return calculateTotalSteps(stepsData?.stepEntries);
  }, [stepsData]);

  const averageSteps = useMemo(() => {
    return calculateAverageSteps(stepsData?.stepEntries);
  }, [stepsData]);

  const highestEntry = useMemo(() => {
    return getHighestStepEntry(stepsData?.stepEntries);
  }, [stepsData]);

  const lowestEntry = useMemo(() => {
    return getLowestStepEntry(stepsData?.stepEntries);
  }, [stepsData]);

  return (
    <main className="app">
      <div className="container">
        <h1>Health Hub</h1>
        <p className="subtitle">Erster React-Durchstich mit sauberer Struktur</p>

        {isLoading && <p>Lade Daten...</p>}

        {errorMessage && <p className="error">{errorMessage}</p>}

        {!isLoading && !errorMessage && stepsData && (
          <>
            <section className="summary-grid">
              <SummaryCard title="Quelle" value={stepsData.source} />
              <SummaryCard title="Export Version" value={stepsData.exportVersion} />
              <SummaryCard title="Record Count" value={formatNumber(stepsData.recordCount)} />
              <SummaryCard title="Total Steps" value={formatNumber(totalSteps)} />
              <SummaryCard title="Average per Day" value={formatNumber(averageSteps)} />
              <SummaryCard
                title="Highest Day"
                value={
                  highestEntry
                  ? `${formatDate(highestEntry.date)} – ${formatNumber(highestEntry.count)}`
                  : '-'
                }
              />
              <SummaryCard
                title="Lowest Day"
                value={
                  lowestEntry
                  ? `${formatDate(lowestEntry.date)} – ${formatNumber(lowestEntry.count)}`
                  : '-'
                }
              />
              <SummaryCard title="Exported At" value={formatDateTime(stepsData.exportedAt)} />
              <SummaryCard title="Imported At" value={formatDateTime(stepsData.importedAt)} />
            </section>

            <StepsTable stepEntries={stepsData.stepEntries} />
          </>
        )}
      </div>
    </main>
  );
}

export default App;