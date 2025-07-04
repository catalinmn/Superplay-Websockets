using System.Net.WebSockets;

namespace Core.Connection;

public class ConnectionData
{
    public WebSocket WebSocket { get; }
    public Guid PlayerId { get; set; }  // Changed to nullable Guid
    public DateTime ConnectedAt { get; } = DateTime.UtcNow;

    public ConnectionData(WebSocket webSocket)
    {
        WebSocket = webSocket;
    }
}