using Entities.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.LetterJam
{
    public interface IPlayerStateRepository : ICommonRepository<PlayerState>
    {
        Task<IList<PlayerState>> GetPlayerStatesForGame(Guid gameId);
        Task<IList<PlayerState>> GetPlayerStatesAndCardsForGame(Guid gameId);
        Task<PlayerState> GetPlayerStateForPlayerId(Guid playerId);
    }
}
