namespace Core.Messages.Responses;

public class LoginResponse: ServerResponse
{
    public Guid PlayerId { get; set; }
    public string DeviceId { get; set; } = "";
    public int InitialCoins { get; set; }
    public int InitialRolls { get; set; }
}