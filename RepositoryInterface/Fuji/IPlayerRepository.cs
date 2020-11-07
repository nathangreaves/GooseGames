using Entities.Fuji;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Fuji
{
    public interface IPlayerInformationRepository : ICommonRepository<PlayerInformation>
    {
        Task<List<PlayerInformation>> GetForGameIncludePlayedCardsAsync(Guid gameId);

        Task<PlayerInformation> GetPlayerInformationFromPlayerIdAndGameId(Guid playerId, Guid gameId);
    }
}
