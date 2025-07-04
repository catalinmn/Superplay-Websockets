import React from 'react';
import ConnectionPanel from './components/ConnectionPanel';
import PlayerDashboard from './components/PlayerDashboard';
import ResourceManager from './components/ResourceManager';
import GiftSender from './components/GiftSender';
import ActivityFeed from './components/ActivityFeed';
import './index.css';
import { GameProvider } from './context/GameContext';

const App: React.FC = () => {
  return (
    <GameProvider>
      <div className="app-container">
        <header>
          <h1>Game Server Client</h1>
        </header>
        
        <main>
          <div className="left-panel">
            <ConnectionPanel />
            <PlayerDashboard />
          </div>
          
          <div className="center-panel">
            <ResourceManager />
            <GiftSender />
          </div>
          
          <div className="right-panel">
            <ActivityFeed />
          </div>
        </main>
        
        <footer>
          <p>Game Server Client &copy; {new Date().getFullYear()}</p>
        </footer>
      </div>
    </GameProvider>
  );
};

export default App;