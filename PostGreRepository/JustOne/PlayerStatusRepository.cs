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

        public async Task UpdatePlayerStatusesForSession(Guid sessionId, Guid sessionStatus, Guid? sessionMasterStatus = null) 
        {
            var statuses = await Context.PlayerStatuses.Where(x => x.Player.SessionId == sessionId).ToListAsync().ConfigureAwait(false);

            if (sessionMasterStatus == null || sessionMasterStatus == sessionStatus)
            {
                foreach (var status in statuses)
                {
                    status.Status = sessionStatus;
                }
            }
            else
            {
                var sessionMasterId = await Context.Sessions.Where(s => s.Id == sessionId).Select(s => s.SessionMasterId).SingleAsync().ConfigureAwait(false);
                foreach (var status in statuses)
                {
                    status.Status = status.PlayerId == sessionMasterId ? sessionMasterStatus.Value : sessionStatus;
                }
            }

            await UpdateRangeAsync(statuses);
        }

        public async Task UpdatePlayerStatusesForRoundAsync(Guid roundId, Guid playerStatus, Guid? activePlayerStatus = null)
        {
            var round = await Context.Rounds.FindAsync(roundId).ConfigureAwait(false);
            
            var statuses = await Context.PlayerStatuses.Where(x => x.Player.SessionId == round.SessionId).ToListAsync().ConfigureAwait(false);

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


        public async Task<PlayerStatus[]> GetPlayerStatusesForSession(Guid sessionId)
        {            
            return await PlayerStatusesForSessionQueryable(sessionId)
                .ToArrayAsync()
                .ConfigureAwait(false);
        }
        public async Task<PlayerStatus[]> GetPlayerStatusesForSessionExceptSessionMaster(Guid sessionId)
        {
            return await PlayerStatusesForSessionQueryable(sessionId)
                .Where(x => x.PlayerId != x.Player.Session.SessionMasterId)
                .ToArrayAsync()
                .ConfigureAwait(false);
        }
        public async Task<PlayerStatus> GetPlayerStatusForSessionMaster(Guid sessionId)
        {
            return await PlayerStatusesForSessionQueryable(sessionId)
                .Where(x => x.PlayerId == x.Player.Session.SessionMasterId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }

        private IQueryable<PlayerStatus> PlayerStatusesForSessionQueryable(Guid sessionId)
        {
            return Context.PlayerStatuses
                            .Include(p => p.Player)
                            .Include(p => p.Player.Session)
                            .AsQueryable()
                            .Where(x => x.Player.SessionId == sessionId);
        }

        public async Task UpdateStatusAsync(Guid playerId, Guid playerStatus)
        {
            var player = await SingleOrDefaultAsync(p => p.PlayerId == playerId);

            player.Status = playerStatus;

            await UpdateAsync(player);
        }
    }
}
