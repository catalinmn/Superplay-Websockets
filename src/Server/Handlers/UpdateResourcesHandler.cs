namespace Server.Handlers;

public class UpdateResourcesHandler : IMessageHandler
{
    public string MessageType => "UpdateResourcesRequest";

    private readonly IResourceService _resourceService;
    private readonly ILogger<UpdateResourcesHandler> _logger;

    public UpdateResourcesHandler(
        IResourceService resourceService,
        ILogger<UpdateResourcesHandler> logger)
    {
        _resourceService = resourceService;
        _logger = logger;
    }

    public async Task<ServerResponse> HandleAsync(
        JsonElement message,
        ConnectionData connection)
    {
        var playerId = connection.PlayerId;

        var request = message.Deserialize<UpdateResourcesRequest>();
        if (request == null)
        {
            return new ErrorResponse("INVALID_REQUEST", "Invalid message format");
        }

        try
        {
            // Directly get new balance from service
            int newBalance = await _resourceService.UpdatePlayerResources(
                playerId,
                request.ResourceType,
                request.ResourceValue);

            _logger.LogInformation(
                "Updated resources for {PlayerId}: {ResourceType} changed by {Amount} to {NewBalance}",
                playerId, request.ResourceType, request.ResourceValue, newBalance);

            return new ResourceUpdateResponse
            {
                ResourceType = request.ResourceType,
                NewBalance = newBalance
            };
        }
        catch (InsufficientResourcesException ex)
        {
            _logger.LogWarning("Insufficient resources for {PlayerId}: {Message}",
                playerId, ex.Message);
            return new ErrorResponse("INSUFFICIENT_RESOURCES", ex.Message);
        }
        catch (PlayerNotFoundException ex)
        {
            _logger.LogWarning("Player not found: {PlayerId}", playerId);
            return new ErrorResponse("PLAYER_NOT_FOUND", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resource update failed for {PlayerId}", playerId);
            return new ErrorResponse("INTERNAL_ERROR", "Resource update failed");
        }
    }
}