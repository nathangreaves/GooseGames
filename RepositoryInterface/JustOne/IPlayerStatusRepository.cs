using Entities.JustOne;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.JustOne
{
    public interface IPlayerStatusRepository : ICommonRepository<PlayerStatus>
    {
        Task UpdatePlayerStatusesForSession(Guid sessionId, Guid sessionStatus, Guid? sessionMasterStatus = null);
        Task UpdatePlayerStatusesForRoundAsync(Guid roundId, Guid playerStatus, Guid? activePlayerStatus = null);
        Task UpdateStatusAsync(Guid playerId, Guid playerStatus);
    }
}
