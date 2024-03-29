﻿using Entities.JustOne;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.JustOne
{
    public interface IResponseVoteRepository : ICommonRepository<ResponseVote>
    {
        Task<Dictionary<Guid, int>> GetNumberOfVotesPerResponseAsync(IEnumerable<Guid> enumerable);
        Task DeleteForPlayerAsync(Guid roundId, Guid playerId); 
        Task DeleteActivePlayerResponseVoteForPlayerAsync(Guid roundId, Guid playerId);
    }
}
