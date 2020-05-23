using Entities.Werewords;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Werewords
{
    public interface IPlayerVoteRepository : ICommonRepository<PlayerVote>
    {
        Task UpsertVoteAsync(PlayerVote playerVote);
    }
}
