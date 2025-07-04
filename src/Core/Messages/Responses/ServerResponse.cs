namespace Core.Messages.Responses;

public abstract class ServerResponse
{
    public string MessageType => GetType().Name;
}