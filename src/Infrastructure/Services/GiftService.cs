using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Exceptions;
using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class GiftService : IGiftService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<GiftService> _logger;

    public GiftService(
        IPlayerRepository playerRepository,
        ILogger<GiftService> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<int> SendGift(
        Guid senderId,
        Guid receiverId,
        ResourceType resourceType,
        int amount)
    {
        if (senderId == receiverId)
            throw new InvalidOperationException("Cannot send gift to yourself");

        if (amount <= 0)
            throw new ArgumentException("Gift amount must be positive", nameof(amount));

        using var transaction = await _playerRepository.BeginTransactionAsync();

        try
        {
            // Get and lock sender
            var sender = await _playerRepository.GetByIdAsync(senderId)
                ?? throw new PlayerNotFoundException(senderId);

            // Get and lock receiver
            var receiver = await _playerRepository.GetByIdAsync(receiverId)
                ?? throw new PlayerNotFoundException(receiverId);

            // Deduct from sender
            var senderBalance = sender.AddResources(resourceType, -amount);

            // Add to receiver
            receiver.AddResources(resourceType, amount);

            // Create gift transaction
            var gift = new GiftTransaction(senderId, receiverId, resourceType, amount);
            await _playerRepository.AddGiftTransactionAsync(gift);

            await _playerRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Gift sent: {SenderId} -> {ReceiverId} ({Amount} {ResourceType})",
                senderId, receiverId, amount, resourceType);

            return senderBalance;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Gift sending failed");
            throw;
        }
    }
}