using Entities.Werewords;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Werewords
{
    public interface IPlayerRoundInformationRepository : ICommonRepository<PlayerRoundInformation>
    {
        Task<IEnumerable<PlayerRoundInformation>> GetForRoundAsync(Guid roundId);
    }
}
