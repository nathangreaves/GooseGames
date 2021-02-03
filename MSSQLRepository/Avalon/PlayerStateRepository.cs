using Entities.Avalon;
using Microsoft.EntityFrameworkCore;
using MSSQLRepository.Contexts;
using RepositoryInterface.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Avalon
{
    public class PlayerStateRepository : CommonRepository<PlayerState>, IPlayerStateRepository
    {
        private AvalonContext Context
        {
            get
            {
                return (AvalonContext)DbContext;
            }
        }

        public PlayerStateRepository(AvalonContext context) : base(context)
        {
        }        

        public async Task<List<PlayerState>> GetForPlayerIds(List<Guid> playerIds)
        {
            return await Context.PlayerStates
                .Include(p => p.GameRole)
                .AsQueryable()
                .Where(p => playerIds.Contains(p.PlayerId))
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
