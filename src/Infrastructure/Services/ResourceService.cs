namespace Infrastructure.Services;

public class ResourceService : IResourceService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<ResourceService> _logger;

    public ResourceService(
        IPlayerRepository playerRepository,
        ILogger<ResourceService> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<int> UpdatePlayerResources(
        Guid playerId,
        ResourceType resourceType,
        int amount)
    {
        try
        {
            // Get player
            var player = await _playerRepository.GetByIdAsync(playerId)
                ?? throw new PlayerNotFoundException(playerId);

            // Update resources
            player.AddResources(resourceType, amount);

            // Save changes
            await _playerRepository.UpdateAsync(player);

            // Return new balance
            return resourceType == ResourceType.Coins ?
                player.Coins : player.Rolls;
        }
        catch (InsufficientResourcesException ex)
        {
            _logger.LogWarning("Resource update failed for {PlayerId}: {Message}",
                playerId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resource update failed for {PlayerId}", playerId);
            throw;
        }
    }
}