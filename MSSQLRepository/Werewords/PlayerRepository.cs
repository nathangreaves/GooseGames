using Entities.Werewords;
using Entities.Werewords.Enums;
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

        private const int MaxPlayers = 10;

        public PlayerRepository(WerewordsContext context) : base(context)
        {
        }
        public async Task DeleteUnreadyPlayersAsync(Guid sessionId)
        {
            var unreadyPlayers = await Context.Players.Where(x => x.SessionId == sessionId && x.Status != PlayerStatusEnum.InLobby).ToListAsync().ConfigureAwait(false);

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
                .OrderBy(x => x).ToListAsync().ConfigureAwait(false);

            return comparisonList.First(c => !reservedPlayerNumbers.Contains(c));
        }
    }
}
