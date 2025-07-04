using Core.Abstractions;
using Core.Connection;
using Core.Domain.Exceptions;
using Core.Messages.Requests;
using Core.Messages.Responses;
using Core.Services;
using Infrastructure.Services;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Server.Handlers;

public class LoginHandler : IMessageHandler
{
    public string MessageType => "LoginRequest";

    private readonly IPlayerService _playerService;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        IPlayerService playerService,
        IConnectionManager connectionManager,
        ILogger<LoginHandler> logger)
    {
        _playerService = playerService;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task<ServerResponse> HandleAsync(
        JsonElement message,
        ConnectionData connection)
    {
        var request = message.Deserialize<LoginRequest>();

        if (string.IsNullOrWhiteSpace(request?.DeviceId))
        {
            _logger.LogWarning("Login request missing DeviceId");
            return new ErrorResponse("INVALID_REQUEST", "Device ID is required");
        }

        if (!IsValidDeviceId(request.DeviceId))
        {
            return new ErrorResponse("INVALID_DEVICE", "Invalid device ID format");
        }


        try
        {
            var player = await _playerService.LoginOrRegisterPlayer(request.DeviceId);

            if (_connectionManager.IsPlayerConnected(player.Id))
            {
                _logger.LogWarning("Player {PlayerId} already connected", player.Id);
                return new ErrorResponse("ALREADY_CONNECTED", "Player is already logged in");
            }

            connection.PlayerId = player.Id;
            _connectionManager.AddConnection(player.Id, connection.WebSocket);

            _logger.LogInformation("Player {PlayerId} logged in", player.Id);

            return new LoginResponse
            {
                PlayerId = player.Id,
                DeviceId = player.DeviceId,
                InitialCoins = player.Coins,
                InitialRolls = player.Rolls
            };
        }
        catch (PlayerNotFoundException ex)
        {
            _logger.LogWarning("Player not found for device {DeviceId}", request.DeviceId);
            return new ErrorResponse("PLAYER_NOT_FOUND", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication  failed for device {DeviceId}", request.DeviceId);
            return new ErrorResponse("AUTH_FAILED", "Authentication  failed");
        }
    }

    private bool IsValidDeviceId(string deviceId)
    {
        // Validate device ID format (e.g., UUID format)
        return Regex.IsMatch(deviceId, @"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$");
    }
}