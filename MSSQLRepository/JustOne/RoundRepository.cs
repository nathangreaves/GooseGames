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
    public class RoundRepository : CommonRepository<Round>, IRoundRepository
    {
        public JustOneContext Context
        {
            get
            {
                return (JustOneContext)DbContext;
            }
        }

        public RoundRepository(JustOneContext context) : base(context)
        {

        }

        public async Task<int> RemoveRoundsAsync(Guid sessionId, int numberOfRoundsToRemove)
        {
            var rounds = await Context.Rounds.Where(r => r.SessionId == sessionId
            && r.ActivePlayerId == null && r.Status == Entities.JustOne.Enums.RoundStatusEnum.New)
                .Take(numberOfRoundsToRemove)
                .ToListAsync();

            int roundsRemoved = 0;
            foreach (var round in rounds)
            {
                roundsRemoved++;
                await DeleteAsync(round);
            }

            return roundsRemoved;
        }

        public async Task<Round> GetCurrentRoundForSessionAsync(Guid sessionId)
        {
            return await Context.Sessions.Where(s => s.Id == sessionId).Select(s => s.CurrentRound).SingleAsync(); ;
        }
    }
}
