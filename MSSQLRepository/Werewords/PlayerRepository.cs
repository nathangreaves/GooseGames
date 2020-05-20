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
    public class PlayerRepository : CommonRepository<Player>, IPlayerRepository
    {
        private WerewordsContext Context => (WerewordsContext)DbContext;

        public PlayerRepository(WerewordsContext context) : base(context)
        {
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
    }
}
