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
    public class PlayerStatusRepository : CommonRepository<PlayerStatus>, IPlayerStatusRepository
    {
        public JustOneContext Context
        {
            get
            {
                return (JustOneContext)DbContext;
            }
        }

        public PlayerStatusRepository(JustOneContext dbContext) : base(dbContext)
        {
        }

        public async Task UpdatePlayerStatusesForGame(Guid gameId, Guid sessionStatus) 
        {
            var statuses = await Context.PlayerStatuses.Where(x => x.GameId == gameId).ToListAsync().ConfigureAwait(false);            
            
            foreach (var status in statuses)
            {
                status.Status = sessionStatus;
            }
            
            await UpdateRangeAsync(statuses);
        }

        public async Task UpdatePlayerStatusesForRoundAsync(Guid roundId, Guid playerStatus, Guid? activePlayerStatus = null)
        {
            var round = await Context.Rounds.FindAsync(roundId).ConfigureAwait(false);
            
            var statuses = await Context.PlayerStatuses.Where(x => x.GameId == round.GameId).ToListAsync().ConfigureAwait(false);

            if (activePlayerStatus == null || activePlayerStatus == playerStatus)
            {
                foreach (var status in statuses)
                {
                    status.Status = playerStatus;
                }
            }
            else
            {
                var activePlayerId = round.ActivePlayerId;
                foreach (var status in statuses)
                {
                    status.Status = status.PlayerId == activePlayerId ? activePlayerStatus.Value : playerStatus;
                }
            }

            await UpdateRangeAsync(statuses);
        }

        public async Task UpdateStatusAsync(Guid playerId, Guid gameId, Guid playerStatus)
        {
            var player = await SingleOrDefaultAsync(p => p.PlayerId == playerId && p.GameId == gameId);

            player.Status = playerStatus;

            await UpdateAsync(player);
        }
    }
}
