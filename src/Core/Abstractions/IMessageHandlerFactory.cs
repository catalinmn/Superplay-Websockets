namespace Core.Abstractions;

public interface IMessageHandlerFactory
{
    IMessageHandler GetHandler(string messageType);
}