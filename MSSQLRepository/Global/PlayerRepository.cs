using Entities.Global;
using Microsoft.EntityFrameworkCore;
using MSSQLRepository.Contexts;
using RepositoryInterface.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Global
{
    public class PlayerRepository : CommonRepository<Player>, IPlayerRepository
    {
        protected GlobalContext Context => (GlobalContext)DbContext;

        public PlayerRepository(GlobalContext context) : base(context)
        {

        }

        public async Task<int> GetNextPlayerNumberAsync(Guid sessionId)
        {
            var highestPlayerNumber = await Context.Players
                .Where(p => p.SessionId == sessionId && p.PlayerNumber != 0)
                .Select(x => x.PlayerNumber)
                .OrderByDescending(x => x).FirstOrDefaultAsync().ConfigureAwait(false);

            return highestPlayerNumber + 1;
        }

        public async Task<IEnumerable<Guid>> DeleteUnreadyPlayersAsync(Guid sessionId)
        {
            var unreadyPlayers = await Context.Players.Where(x => x.SessionId == sessionId && x.Name == null && x.PlayerNumber <= 0).ToListAsync().ConfigureAwait(false);

            var returnList = unreadyPlayers.Select(p => p.Id).ToList();

            if (unreadyPlayers.Any())
            {
                foreach (var unreadyPlayer in unreadyPlayers)
                {
                    await DeleteAsync(unreadyPlayer).ConfigureAwait(false);
                }
            }

            return returnList;
        }
    }
}
