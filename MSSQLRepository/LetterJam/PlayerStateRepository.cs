using Entities.LetterJam;
using Microsoft.EntityFrameworkCore;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.LetterJam
{
    public class PlayerStateRepository : CommonRepository<PlayerState>, IPlayerStateRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public PlayerStateRepository(LetterJamContext context)
            : base(context)
        {

        }

        public async Task<IList<PlayerState>> GetPlayerStatesForGame(Guid gameId)
        {
            return await Context.PlayerStates
                .Where(p => p.GameId == gameId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IList<PlayerState>> GetPlayerStatesAndCardsForGame(Guid gameId)
        {
            return await Context.PlayerStates.AsQueryable()
                    .Include(p => p.CurrentLetter)
                   .Where(p => p.GameId == gameId)
                   .ToListAsync()
                   .ConfigureAwait(false);
        }

        public async Task<PlayerState> GetPlayerStateForPlayerId(Guid playerId)
        {
            return await SingleOrDefaultAsync(p => p.PlayerId == playerId);
        }
    }
}
