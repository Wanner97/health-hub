import { useState } from 'react';
import ActivityDaysPage from './components/activityDays/ActivityDaysPage';
import HeartRateDaysPage from './components/heartRateDays/HeartRateDaysPage';
import HomeDashboard from './components/home/HomeDashboard';
import ImportBatchesPage from './components/importBatches/ImportBatchesPage';
import SleepSessionsPage from './components/sleepSessions/SleepSessionsPage';
import BloodOxygenDaysPage from './components/bloodOxygenDays/BloodOxygenDaysPage';
import WeightMeasurementsPage from './components/weightMeasurements/WeightMeasurementsPage';
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
          <HomeDashboard onSelectSection={setCurrentSection} />
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

        {currentSection === APP_SECTIONS.HEART_RATE_DAYS && (
          <HeartRateDaysPage onBack={goHome} />
        )}

        {currentSection === APP_SECTIONS.BLOOD_OXYGEN_DAYS && (
          <BloodOxygenDaysPage onBack={goHome} />
        )}

        {currentSection === APP_SECTIONS.WEIGHT_MEASUREMENTS && (
          <WeightMeasurementsPage onBack={goHome} />
        )}
      </div>
    </main>
  );
}

export default App;