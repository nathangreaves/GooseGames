using Entities.JustOne;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.JustOne
{
    public class ResponseRepository : CommonRepository<Response>, IResponseRepository
    {
        public JustOneContext Context
        {
            get
            {
                return (JustOneContext)DbContext;
            }
        }

        public ResponseRepository(JustOneContext context) : base(context)
        {

        }

        public async Task<bool> AllPlayersHaveResponded(Round round)
        {
            var gameId = round.GameId;
            var roundId = round.Id;
            var activePlayerId = round.ActivePlayerId.Value;

            var  countOfPlayersThatHaveNoResponse = await Context.PlayerStatuses
                .Where(p => p.GameId == gameId && p.PlayerId != activePlayerId)
                .Where(p => !Context.Responses.Any(r => r.RoundId == roundId && r.PlayerId == p.PlayerId))
                .CountAsync().ConfigureAwait(false);

            return countOfPlayersThatHaveNoResponse == 0;
        }

        public async Task DeleteForPlayerAsync(Guid roundId, Guid playerId)
        {
            var response = await Context.Responses
                .Where(r => r.RoundId == roundId && r.PlayerId == playerId)
                .SingleOrDefaultAsync();

            if (response != null)
            {
                await DeleteAsync(response);
            }
        }
    }
}
