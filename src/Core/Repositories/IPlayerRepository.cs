namespace Core.Repositories;

public interface IPlayerRepository
{
    Task<Player> GetByDeviceIdAsync(string deviceId);
    Task<Player> GetByIdAsync(Guid playerId);
    Task AddAsync(Player player);
    Task UpdateAsync(Player player);
    Task AddGiftTransactionAsync(GiftTransaction transaction);
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}