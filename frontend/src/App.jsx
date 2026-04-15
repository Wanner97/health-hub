import { useState } from 'react';
import ActivityDaysPage from './components/activityDays/ActivityDaysPage';
import HomeSectionSelector from './components/home/HomeSectionSelector';
import ImportBatchesPage from './components/importBatches/ImportBatchesPage';
import SleepSessionsPage from './components/sleepSessions/SleepSessionsPage';
import { APP_SECTIONS } from './constants/appSections';

function App() {
  const [currentSection, setCurrentSection] = useState(APP_SECTIONS.HOME);

  function goHome() {
    setCurrentSection(APP_SECTIONS.HOME);
  }

  return (
    <main className="app">
      <div className="container">
        {currentSection === APP_SECTIONS.HOME && (
          <HomeSectionSelector onSelectSection={setCurrentSection} />
        )}

        {currentSection === APP_SECTIONS.ACTIVITY_DAYS && (
          <ActivityDaysPage onBack={goHome} />
        )}

        {currentSection === APP_SECTIONS.IMPORT_BATCHES && (
          <ImportBatchesPage onBack={goHome} />
        )}

        {currentSection === APP_SECTIONS.SLEEP_SESSIONS && (
          <SleepSessionsPage onBack={goHome} />
        )}
      </div>
    </main>
  );
}

export default App;