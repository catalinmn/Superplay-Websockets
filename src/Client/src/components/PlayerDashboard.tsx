import React from 'react';
import { useGameContext } from '../context/GameContext';

const PlayerDashboard: React.FC = () => {
  const { player } = useGameContext();

  if (!player) return null;

  return (
    <div className="dashboard">
      <div className="resource-card">
        <h3>Coins</h3>
        <div className="resource-value">{player.coins}</div>
      </div>
      <div className="resource-card">
        <h3>Rolls</h3>
        <div className="resource-value">{player.rolls}</div>
      </div>
      <div className="player-info-card">
        <h3>Player Info</h3>
        <p><strong>Device ID:</strong> {player.deviceId}</p>
        <p><strong>Player ID:</strong> {player.id}</p>
      </div>
    </div>
  );
};

export default PlayerDashboard;