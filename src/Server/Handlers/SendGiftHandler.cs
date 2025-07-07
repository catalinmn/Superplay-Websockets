namespace Server.Handlers;

public class SendGiftHandler : IMessageHandler
{
    public string MessageType => "SendGiftRequest";

    private readonly IGiftService _giftService;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<SendGiftHandler> _logger;

    public SendGiftHandler(
        IGiftService giftService,
        IConnectionManager connectionManager,
        ILogger<SendGiftHandler> logger)
    {
        _giftService = giftService;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task<ServerResponse> HandleAsync(
        JsonElement message,
        ConnectionData connection)
    {
        var senderId = connection.PlayerId;

        var request = message.Deserialize<SendGiftRequest>();
        if (request == null)
        {
            return new ErrorResponse("INVALID_REQUEST", "Invalid message format");
        }

        try
        {
            var senderBalance = await _giftService.SendGift(
                senderId,
                request.FriendPlayerId,
                request.ResourceType,
                request.ResourceValue);

            _logger.LogInformation(
                "Gift sent: {SenderId} -> {ReceiverId} ({Amount} {ResourceType})",
                senderId, request.FriendPlayerId,
                request.ResourceValue, request.ResourceType);

            await NotifyFriendIfOnline(
                request.FriendPlayerId,
                senderId,
                request.ResourceType,
                request.ResourceValue);

            return new ResourceUpdateResponse
            {
                ResourceType = request.ResourceType,
                NewBalance = senderBalance
            };
        }
        catch (PlayerNotFoundException ex)
        {
            _logger.LogWarning("Player not found: {Message}", ex.Message);
            return new ErrorResponse("PLAYER_NOT_FOUND", ex.Message);
        }
        catch (InsufficientResourcesException ex)
        {
            _logger.LogWarning("Insufficient resources: {Message}", ex.Message);
            return new ErrorResponse("INSUFFICIENT_RESOURCES", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid gift operation: {Message}", ex.Message);
            return new ErrorResponse("INVALID_OPERATION", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gift sending failed");
            return new ErrorResponse("INTERNAL_ERROR", "Gift sending failed");
        }
    }

    private async Task NotifyFriendIfOnline(
        Guid friendPlayerId,
        Guid senderId,
        ResourceType resourceType,
        int amount)
    {
        var friendSocket = _connectionManager.GetConnection(friendPlayerId);
        if (friendSocket == null || friendSocket.State != WebSocketState.Open)
            return;

        try
        {
            var giftEvent = new GiftEvent
            {
                FromPlayerId = senderId,
                ResourceType = resourceType,
                Amount = amount
            };

            var jsonResponse = JsonSerializer.Serialize(giftEvent);
            var bytes = Encoding.UTF8.GetBytes(jsonResponse);

            await friendSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            _logger.LogInformation(
                "Gift notification sent to {PlayerId}", friendPlayerId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to send gift notification to {PlayerId}", friendPlayerId);
        }
    }
}