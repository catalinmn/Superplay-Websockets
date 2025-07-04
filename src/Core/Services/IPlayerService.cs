using Core.Domain.Entities;

namespace Core.Services;

public interface IPlayerService
{
    Task<Player> LoginOrRegisterPlayer(string deviceId);
    Task<Player> GetPlayerById(Guid playerId);
}