using Entities.JustOne;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.JustOne
{
    public interface IPlayerStatusRepository : ICommonRepository<PlayerStatus>
    {
        Task UpdatePlayerStatusesForGame(Guid gameId, Guid sessionStatus);
        Task UpdatePlayerStatusesForRoundAsync(Guid roundId, Guid playerStatus, Guid? activePlayerStatus = null);
        Task UpdateStatusAsync(Guid playerId, Guid gameId, Guid playerStatus);
    }
}
