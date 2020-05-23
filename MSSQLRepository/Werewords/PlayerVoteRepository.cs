using Entities.Werewords;
using Microsoft.EntityFrameworkCore;
using MSSQLRepository.Contexts;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Werewords
{
    public class PlayerVoteRepository : CommonRepository<PlayerVote>, IPlayerVoteRepository
    {
        private WerewordsContext Context => (WerewordsContext)DbContext;

        public PlayerVoteRepository(WerewordsContext context) : base(context)
        {

        }

        public async Task UpsertVoteAsync(PlayerVote playerVote)
        {
            var vote = await Context.PlayerVotes.Where(p => p.PlayerId == playerVote.PlayerId && p.RoundId == playerVote.RoundId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (vote != null)
            {
                vote.VotedPlayerId = playerVote.VotedPlayerId;
                vote.VoteType = playerVote.VoteType;
                vote.CreatedUtc = DateTime.UtcNow;
                await UpdateAsync(vote);
            }
            else
            {
                await InsertAsync(playerVote);
            }
        }
    }
}
