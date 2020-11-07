using Entities.Fuji.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Fuji
{
    public interface IDeckCardRepository : ICommonRepository<DeckCard>
    {
        Task<DeckCard> GetNextCardAsync(Guid gameId);
    }
}
