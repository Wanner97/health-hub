import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './styles/tokens.css';
import './styles/base.css';
import './styles/common.css';
import './styles/activityDays.css';
import './styles/appSections.css';
import './styles/importBatches.css';
import './styles/sleepSessions.css';
import './styles/heartRateDays.css';
import './styles/bloodOxygenDays.css';
import App from './App.jsx';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
);