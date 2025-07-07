namespace Core.Services;

public interface IResourceService
{
    Task<int> UpdatePlayerResources(
        Guid playerId,
        ResourceType resourceType,
        int amount);
}