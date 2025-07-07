import React, { createContext, useState, useContext } from 'react';
import { GameWebSocket } from '../services/websocket';
import type { ActivityLogEntry, GameContextType, LoginRequest, Player, ServerResponse } from '../types/gameTypes';

const GameContext = createContext<GameContextType | undefined>(undefined);

export const useGameContext = () => {
  const context = useContext(GameContext);
  if (!context) {
    throw new Error('useGameContext must be used within a GameProvider');
  }
  return context;
};

export const GameProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [player, setPlayer] = useState<Player | null>(null);
  const [connection, setConnection] = useState<GameWebSocket | null>(null);
  const [activityLog, setActivityLog] = useState<ActivityLogEntry[]>([]);
  const [error, setError] = useState<string | null>(null);

  const connectToGame = (deviceId: string) => {
    const ws = new GameWebSocket(`ws://localhost:5000/ws`);

    ws.on('open', () => {
      setConnection(ws);
      addActivity('Connected to game server');

      // Send login request after connection
      const loginRequest: LoginRequest = {
        MessageType: "LoginRequest",
        DeviceId: deviceId
      };
      ws.send(loginRequest);
      addActivity(`Sent: ${JSON.stringify(loginRequest)}`);
    });

    ws.on('message', (message: ServerResponse) => {
      handleGameMessage(message);
    });

    ws.on('close', () => {
      setConnection(null);
      addActivity('Disconnected from game server');
    });

    ws.on('error', (err: Event) => {
      setError('Connection error: ' + err.type);
    });
  };

  const handleGameMessage = (message: ServerResponse) => {
    addActivity(`Received: ${JSON.stringify(message)}`);

    switch (message.MessageType) {
      case 'LoginResponse':
        setPlayer({
          id: message.PlayerId,
          deviceId: message.DeviceId,
          coins: message.InitialCoins,
          rolls: message.InitialRolls
        });
        break;

      case 'ResourceUpdateResponse':
        setPlayer(prev => prev ? {
          ...prev,
          coins: message.ResourceType === 0 ? message.NewBalance : prev.coins,
          rolls: message.ResourceType === 1 ? message.NewBalance : prev.rolls
        } : null);
        break;

      case 'GiftEvent':
        setPlayer(prev => prev ? {
          ...prev,
          coins: message.ResourceType === 0 ? prev.coins + message.Amount : prev.coins,
          rolls: message.ResourceType === 1 ? prev.rolls + message.Amount : prev.rolls
        } : null);
        addActivity(`Received gift: ${message.Amount} ${message.ResourceType === 0 ? 'coins' : 'rolls'} from ${message.FromPlayerId}`);
        break;

      case 'ErrorResponse':
        setError(`${message.ErrorCode}: ${message.Message}`);
        break;

      default:
        break;
    }
  };

  const addActivity = (text: string) => {
    setActivityLog(prev => [
      ...prev,
      { timestamp: new Date().toISOString(), text }
    ]);
  };

  const sendMessage = (message: any) => {
    if (connection && connection.readyState === WebSocket.OPEN) {
      connection.send(message);
      addActivity(`Sent: ${JSON.stringify(message)}`);
      return true;
    }
    setError('Not connected to game server');
    return false;
  };

  const updateResources = (resourceType: number, amount: number) => {
    const message = {
      MessageType: "UpdateResourcesRequest",
      ResourceType: resourceType,
      ResourceValue: amount
    };
    sendMessage(message);
  };

  const sendGift = (friendPlayerId: string, resourceType: number, amount: number) => {
    const message = {
      MessageType: "SendGiftRequest",
      FriendPlayerId: friendPlayerId,
      ResourceType: resourceType,
      ResourceValue: amount
    };
    sendMessage(message);
  };

  const login = async (deviceId: string) => {
    try {
      setError(null);
      connectToGame(deviceId);
      return true;
    } catch (err) {
      setError('Authentication failed');
      return false;
    }
  };

  const logout = () => {
    if (connection) {
      connection.close();
      setConnection(null);
    }
    setPlayer(null);
    setActivityLog([]);
  };

  const clearError = () => setError(null);

  return (
    <GameContext.Provider value={{
      player,
      connection,
      activityLog,
      error,
      login,
      logout,
      updateResources,
      sendGift,
      addActivity,
      clearError
    }}>
      {children}
    </GameContext.Provider>
  );
};