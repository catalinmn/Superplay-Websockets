namespace Core.Domain.Exceptions;

public class PlayerAlreadyConnectedException : Exception
{
    public PlayerAlreadyConnectedException(Guid playerId)
        : base($"Player {playerId} is already connected")
    {
    }
}