using Core.Domain.Enums;

namespace Core.Messages.Responses;

public class GiftEvent : ServerResponse
{
    public Guid FromPlayerId { get; set; }
    public ResourceType ResourceType { get; set; }
    public int Amount { get; set; }
}