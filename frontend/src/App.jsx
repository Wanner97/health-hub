import { useEffect, useMemo, useState } from 'react';
import './App.css';
import { getLatestSteps } from './api/stepsApi';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

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
    if (!stepsData?.stepEntries) {
      return 0;
    }

    return stepsData.stepEntries.reduce((sum, entry) => sum + entry.count, 0);
  }, [stepsData]);

  return (
    <main className="app">
      <div className="container">
        <h1>Health Hub</h1>
        <p className="subtitle">Erster React-Durchstich mit Backend-Anbindung</p>

        {isLoading && <p>Lade Daten...</p>}

        {errorMessage && <p className="error">{errorMessage}</p>}

        {!isLoading && !errorMessage && stepsData && (
          <>
            <section className="summary-grid">
              <article className="card">
                <h2>Quelle</h2>
                <p>{stepsData.source}</p>
              </article>

              <article className="card">
                <h2>Export Version</h2>
                <p>{stepsData.exportVersion}</p>
              </article>

              <article className="card">
                <h2>Record Count</h2>
                <p>{stepsData.recordCount}</p>
              </article>

              <article className="card">
                <h2>Total Steps</h2>
                <p>{totalSteps}</p>
              </article>

              <article className="card">
                <h2>Exported At</h2>
                <p>{new Date(stepsData.exportedAt).toLocaleString()}</p>
              </article>

              <article className="card">
                <h2>Imported At</h2>
                <p>{new Date(stepsData.importedAt).toLocaleString()}</p>
              </article>
            </section>

            <section className="table-section">
              <h2>Step Entries</h2>

              <table>
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Count</th>
                    <th>Start Time</th>
                    <th>End Time</th>
                  </tr>
                </thead>
                <tbody>
                  {stepsData.stepEntries.map((entry) => (
                    <tr key={entry.date}>
                      <td>{entry.date}</td>
                      <td>{entry.count}</td>
                      <td>{new Date(entry.startTime).toLocaleString()}</td>
                      <td>{new Date(entry.endTime).toLocaleString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </section>
          </>
        )}
      </div>
    </main>
  );
}

export default App;