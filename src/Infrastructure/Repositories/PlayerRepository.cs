using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<PlayerRepository> _logger;

    public PlayerRepository(AppDbContext context, ILogger<PlayerRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Player> GetByDeviceIdAsync(string deviceId)
    {
        var player = await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.DeviceId == deviceId);

        //if (player == null)
        //{
        //    throw new PlayerNotFoundException($"Player with DeviceId {deviceId} not found.");
        //}

        return player;
    }

    public async Task<Player> GetByIdAsync(Guid playerId)
    {
        var player = await _context.Players.FindAsync(playerId);
        if (player == null)
        {
            throw new PlayerNotFoundException($"Player with Id {playerId} not found.");
        }
        return player;
    }

    public async Task AddAsync(Player player)
    {
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created player {PlayerId} for device {DeviceId}",
            player.Id, player.DeviceId);
    }

    public async Task UpdateAsync(Player player)
    {
        _context.Players.Update(player);
        await _context.SaveChangesAsync();
        _logger.LogDebug("Updated player {PlayerId}", player.Id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
        _logger.LogTrace("Saved repository changes");
    }

    public async Task AddGiftTransactionAsync(GiftTransaction transaction)
    {
        await _context.GiftTransactions.AddAsync(transaction);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        return transaction; 
    }
}