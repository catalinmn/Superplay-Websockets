using Core.Messages.Responses;

namespace Core.Abstractions;

public interface IMessageHandlerFactory
{
    IMessageHandler GetHandler(string messageType);
}