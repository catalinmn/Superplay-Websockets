using Core.Abstractions;
using Core.Connection;
using Core.Messages.Responses;
using Infrastructure.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Middleware;

public class WebSocketMiddleware: IMiddleware
{
    private readonly IMessageHandlerFactory _handlerFactory;
    private readonly ILogger<WebSocketMiddleware> _logger;
    private readonly IConnectionManager _connectionManager;

    public WebSocketMiddleware(
        IMessageHandlerFactory handlerFactory,
        ILogger<WebSocketMiddleware> logger,
        IConnectionManager connectionManager)
    {
        _handlerFactory = handlerFactory;
        _logger = logger;
        _connectionManager = connectionManager;
    }


    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var connection = new ConnectionData(webSocket);

            try
            {
                await HandleWebSocketConnection(connection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket connection error");
            }
            finally
            {
                _connectionManager.RemoveConnection(connection.PlayerId);

                await webSocket.CloseAsync(
                  WebSocketCloseStatus.NormalClosure,
                  string.Empty,
                  CancellationToken.None);
            }
        }
        else
        {
            await next(context);
        }
    }

    private async Task HandleWebSocketConnection(
        ConnectionData connection)
    {
        var buffer = new byte[1024 * 4];
        var webSocket = connection.WebSocket;

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {    
                return;
            }

            try
            {
                var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var messageJson = JsonDocument.Parse(messageString).RootElement;

                if (!messageJson.TryGetProperty("MessageType", out var messageTypeProperty))
                {
                    await SendErrorResponse(webSocket, "Missing MessageType property");
                    continue;
                }

                var messageType = messageTypeProperty.GetString();
                if (string.IsNullOrEmpty(messageType))
                {
                    await SendErrorResponse(webSocket, "Invalid MessageType");
                    continue;
                }

                var handler = _handlerFactory.GetHandler(messageType);
                var response = await handler.HandleAsync(messageJson, connection);
                await SendResponse(webSocket, response);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning("Invalid JSON format: {Message}", ex.Message);
                await SendErrorResponse(webSocket, "Invalid message format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await SendErrorResponse(webSocket, "Internal server error");
            }
        }
    }

    private async Task SendResponse(WebSocket webSocket, ServerResponse response)
    {
        var jsonResponse = JsonSerializer.Serialize(response, response.GetType());

        var bytes = Encoding.UTF8.GetBytes(jsonResponse);
        await webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    private async Task SendErrorResponse(WebSocket webSocket, string message)
    {
        await SendResponse(webSocket, new ErrorResponse("INVALID_REQUEST", message));
    }
}