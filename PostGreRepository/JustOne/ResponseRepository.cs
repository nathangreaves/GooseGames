using Entities.JustOne;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostGreRepository.JustOne
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
            var sessionId = round.SessionId;
            var roundId = round.Id;
            var activePlayerId = round.ActivePlayerId.Value;

            var  countOfPlayersThatHaveNoResponse = await Context.Players
                .Where(p => p.SessionId == sessionId && p.Id != activePlayerId)
                .Where(p => !Context.Responses.Any(r => r.RoundId == roundId && r.PlayerId == p.Id))
                .CountAsync().ConfigureAwait(false);

            return countOfPlayersThatHaveNoResponse == 0;
        }
    }
}
