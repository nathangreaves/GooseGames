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
    public class PlayerRoundInformationRepository : CommonRepository<PlayerRoundInformation>, IPlayerRoundInformationRepository
    {
        private WerewordsContext Context => (WerewordsContext)DbContext;

        public PlayerRoundInformationRepository(WerewordsContext context) : base(context)
        {

        }

        public async Task<IEnumerable<PlayerRoundInformation>> GetForRoundAsync(Guid roundId)
        {
            return await Context.PlayerRoundInformation
                .Include(p => p.Player)
                .Where(p => p.RoundId == roundId)
                .OrderBy(p => p.Player.PlayerNumber)
                .ToListAsync()
                .ConfigureAwait(false);
        }


        public async Task<PlayerRoundInformation> GetMayorAsync(Guid roundId)
        {
            return await Context.PlayerRoundInformation
                .Include(p => p.Player)
                .Where(p => p.RoundId == roundId && p.IsMayor)                
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<PlayerRoundInformation> GetForPlayerAndRoundAsync(Guid roundId, Guid playerId)
        {
            return await Context.PlayerRoundInformation
                .Include(p => p.Player)
                .Where(p => p.RoundId == roundId && p.PlayerId == playerId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}
