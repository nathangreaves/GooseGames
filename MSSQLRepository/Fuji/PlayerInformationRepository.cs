using Entities.Fuji;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Fuji
{
    public class PlayerInformationRepository : CommonRepository<PlayerInformation>, IPlayerInformationRepository
    {
        public FujiContext Context
        {
            get
            {
                return (FujiContext)DbContext;
            }
        }

        public PlayerInformationRepository(FujiContext context) : base(context)
        {

        }

        public async Task<List<PlayerInformation>> GetForGameIncludePlayedCardsAsync(Guid gameId)
        {
            return await Context.PlayerInformation.Include(p => p.PlayedCard).Where(p => p.GameId == gameId).ToListAsync().ConfigureAwait(false);
        }

        public async Task<PlayerInformation> GetPlayerInformationFromPlayerIdAndGameId(Guid playerId, Guid gameId)
        {
            return await Context.PlayerInformation
                .SingleOrDefaultAsync(p => p.PlayerId == playerId && p.GameId == gameId)
                .ConfigureAwait(false);
        }
    }
}
