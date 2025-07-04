using System.Text.Json;
using Core.Messages.Responses;
using Core.Connection;

namespace Core.Abstractions;

public interface IMessageHandler
{
    string MessageType { get; }
    Task<ServerResponse> HandleAsync(JsonElement message, ConnectionData connection);
}