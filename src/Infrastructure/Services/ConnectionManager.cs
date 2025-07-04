using Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Infrastructure.Services;

public class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _connections = new();
    private readonly ILogger<ConnectionManager> _logger;

    public ConnectionManager(ILogger<ConnectionManager> logger)
    {
        _logger = logger;
    }

    public void AddConnection(Guid playerId, WebSocket socket)
    {
        if (_connections.TryAdd(playerId, socket))
        {
            _logger.LogInformation("Player {playerId} connected", playerId);
        }
        else
        {
            _logger.LogWarning("Player {playerId} already connected", playerId);
        }
    }

    public void RemoveConnection(Guid playerId)
    {
        if (_connections.TryRemove(playerId, out _))
        {
            _logger.LogInformation("Player {playerId} disconnected", playerId);
        }
    }

    public WebSocket GetConnection(Guid playerId)
    {
        if (_connections.TryGetValue(playerId, out var socket))
        {
            return socket;
        }

        throw new KeyNotFoundException($"No connection found for playerId: {playerId}");
    }

    public bool IsPlayerConnected(Guid playerId)
    {
        return _connections.ContainsKey(playerId);
    }
}