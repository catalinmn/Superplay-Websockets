namespace Core.Domain.Exceptions;

public class PlayerNotFoundException : Exception
{
    public Guid? PlayerId { get; }
    public string? DeviceId { get; } 

    public PlayerNotFoundException(Guid playerId)
        : base($"Player with ID '{playerId}' was not found")
    {
        PlayerId = playerId;
        DeviceId = null; 
    }

    public PlayerNotFoundException(string deviceId)
        : base($"Player with Device ID '{deviceId}' was not found")
    {
        DeviceId = deviceId;
        PlayerId = null;
    }
}