namespace Server.Handlers;

public class MessageHandlerFactory : IMessageHandlerFactory
{
    private readonly IEnumerable<IMessageHandler> _handlers;

    public MessageHandlerFactory(IEnumerable<IMessageHandler> handlers)
    {
        _handlers = handlers;
    }

    public IMessageHandler GetHandler(string messageType)
    {
        var handler = _handlers.FirstOrDefault(h => h.MessageType == messageType);
        if (handler == null)
            throw new InvalidOperationException($"No handler for message type: {messageType}");
        return handler;
    }
}