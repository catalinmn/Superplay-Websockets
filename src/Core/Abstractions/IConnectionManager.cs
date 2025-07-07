
namespace Core.Abstractions;

public interface IConnectionManager
{
    void AddConnection(Guid playerId, WebSocket socket);
    void RemoveConnection(Guid playerId);
    WebSocket GetConnection(Guid playerId);
    bool IsPlayerConnected(Guid playerId);
}