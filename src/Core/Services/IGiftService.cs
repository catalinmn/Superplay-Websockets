using Core.Domain.Enums;

namespace Core.Services;

public interface IGiftService
{
    Task<int> SendGift(
        Guid senderId,
        Guid receiverId,
        ResourceType resourceType,
        int amount);
}