using Entities.JustOne;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.JustOne
{
    public interface IResponseRepository : ICommonRepository<Response>
    {
        Task<bool> AllPlayersHaveResponded(Round round);
        Task DeleteForPlayerAsync(Guid roundId, Guid playerId);
    }
}
