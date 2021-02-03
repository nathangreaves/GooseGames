using Entities.Avalon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Avalon
{
    public interface IPlayerStateRepository : ICommonRepository<PlayerState>
    {
        Task<List<PlayerState>> GetForPlayerIds(List<Guid> playerIds);
    }
}
