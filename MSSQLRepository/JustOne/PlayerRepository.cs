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
    public class PlayerRepository : CommonRepository<Player>, IPlayerRepository
    {
        private const int MaxPlayers = 7;

        public JustOneContext Context
        {
            get
            {
                return (JustOneContext)DbContext;
            }
        }

        public PlayerRepository(JustOneContext context) : base (context)
        {

        }

        public async Task<string> GetActivePlayerConnectionIdAsync(Guid roundId)
        {
            return await Context.Rounds
                .AsQueryable().Where(r => r.Id == roundId)
                .Select(r => r.ActivePlayer.ConnectionId).SingleAsync().ConfigureAwait(false);
        }

        public async Task DeleteUnreadyPlayersAsync(Guid sessionId)
        {
            var unreadyPlayers = await Context.Players.Where(x => x.SessionId == sessionId && x.Name == null && x.PlayerNumber <= 0).ToListAsync().ConfigureAwait(false);

            if (unreadyPlayers.Any())
            {
                foreach (var unreadyPlayer in unreadyPlayers)
                {
                    await DeleteAsync(unreadyPlayer).ConfigureAwait(false);
                }
            }
        }

        public async Task<int> GetNextPlayerNumberAsync(Guid sessionId)
        {
            var comparisonList = new List<int>();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                comparisonList.Add(i);
            }

            var reservedPlayerNumbers = await Context.Players
                .Where(p => p.SessionId == sessionId && p.PlayerNumber != 0)
                .Select(x => x.PlayerNumber)
                .OrderBy(x => x).ToListAsync();

            return comparisonList.First(c => !reservedPlayerNumbers.Contains(c));
        }
    }
}
