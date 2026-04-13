import ImportBatchList from './ImportBatchList';
import { useImportBatches } from '../../hooks/useImportBatches';

function ImportBatchesPage({ onBack }) {
  const { batches, isLoading, errorMessage } = useImportBatches();

  return (
    <section>
      <div className="page-header">
        <button type="button" className="back-button" onClick={onBack}>
          ← Zurück zur Startseite
        </button>
      </div>

      <h1>Import-Batches</h1>
      <p className="subtitle">
        Übersicht über importierte Datenpakete und deren Verarbeitung.
      </p>

      {isLoading && <p>Lade Import-Batches...</p>}
      {errorMessage && <p className="error">{errorMessage}</p>}

      {!isLoading && !errorMessage && (
        <ImportBatchList batches={batches} />
      )}
    </section>
  );
}

export default ImportBatchesPage;