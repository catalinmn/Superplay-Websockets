namespace Infrastructure.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger _logger;

    public PlayerService(IPlayerRepository playerRepository, 
        ILogger<PlayerService> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<Player> LoginOrRegisterPlayer(string deviceId)
    {
        // Try to find existing player
        var player = await _playerRepository.GetByDeviceIdAsync(deviceId);

        if (player != null)
        {
            _logger.LogInformation("Player {PlayerId} logged in", player.Id);
            return player;
        }

        // Create new player
        player = new Player(deviceId);
        await _playerRepository.AddAsync(player);

        _logger.LogInformation("Registered new player {PlayerId} for device {DeviceId}",
            player.Id, deviceId);

        return player;
    }

    public async Task<Player> GetPlayerById(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);

        if (player == null)
            throw new PlayerNotFoundException(playerId);

        return player;
    }


}