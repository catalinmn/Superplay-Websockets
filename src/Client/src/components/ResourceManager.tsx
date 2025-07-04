import React, { useState } from 'react';
import { useGameContext } from '../context/GameContext';

const ResourceManager: React.FC = () => {
  const { updateResources, player } = useGameContext();
  const [resourceType, setResourceType] = useState<number>(0);
  const [amount, setAmount] = useState<string>('10');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const numAmount = parseInt(amount, 10);
    if (!isNaN(numAmount)) {
      updateResources(resourceType, numAmount);
    }
  };

  if (!player) return null;

  return (
    <div className="resource-manager">
      <h3>Manage Resources</h3>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Resource Type:</label>
          <select
            value={resourceType}
            onChange={(e) => setResourceType(parseInt(e.target.value, 10))}
          >
            <option value={0}>Coins</option>
            <option value={1}>Rolls</option>
          </select>
        </div>
        <div className="form-group">
          <label>Amount:</label>
          <input
            type="number"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            min={resourceType === 0 ? -player.coins : -player.rolls}
            max={resourceType === 0 ? 1000 : 100}
            required
          />
          <small>
            {resourceType === 0 ? 
              `Range: ${-player.coins} to 1000` : 
              `Range: ${-player.rolls} to 100`}
          </small>
        </div>
        <button type="submit" className="action-btn">
          Update Resources
        </button>
      </form>
    </div>
  );
};

export default ResourceManager;