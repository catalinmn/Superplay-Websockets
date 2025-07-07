namespace Core.Messages.Requests;

public class SendGiftRequest
{
    public Guid FriendPlayerId { get; set; }
    public ResourceType ResourceType { get; set; }
    public int ResourceValue { get; set; }
}