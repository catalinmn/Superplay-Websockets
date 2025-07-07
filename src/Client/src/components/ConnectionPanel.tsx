import React, { useState } from 'react';
import { useGameContext } from '../context/GameContext';
import { v4 as uuidv4 } from 'uuid';

const ConnectionPanel: React.FC = () => {
    const { login, logout, player, error, clearError } = useGameContext();
    const [deviceId, setDeviceId] = useState<string>('');

    const generateDeviceId = () => {
        const deviceId = uuidv4();
        return deviceId;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        clearError();
        await login(deviceId);
    };

    if (player) {
        return (
            <div className="connection-panel">
                <div className="player-info">
                    <h3>Connected as: {player.deviceId}</h3>
                    <p>Player ID: {player.id}</p>
                </div>
                <button onClick={logout} className="logout-btn">
                    Logout
                </button>
            </div>
        );
    }

    return (
        <div className="connection-panel">
            <h2>Device Authentication</h2>
            {error && <div className="error">{error}</div>}
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label>Device ID:</label>
                    <div className="input-group">
                        <input
                            type="text"
                            value={deviceId}
                            onChange={(e) => setDeviceId(e.target.value)}
                            placeholder="Enter your device ID"
                            required
                        />
                        <button
                            type="button"
                            className="generate-id-btn"
                            onClick={() => setDeviceId(generateDeviceId())}
                        >
                            Generate ID
                        </button>
                    </div>
                </div>
                <button type="submit" className="auth-btn">
                    Connect
                </button>
            </form>
            <div className="info-note">
                <p>Enter a unique device ID or generate a random one.</p>
                <p>This ID will be used to identify your player account.</p>
            </div>
        </div>
    );
};

export default ConnectionPanel;