using Entities.Fuji.Cards;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Fuji
{
    public class DeckCardRepository : CommonRepository<DeckCard>, IDeckCardRepository
    {
        public FujiContext Context
        {
            get
            {
                return (FujiContext)DbContext;
            }
        }

        public DeckCardRepository(FujiContext dbContext) : base(dbContext)
        {
        }

        public async Task<DeckCard> GetNextCardAsync(Guid sessionId)
        {
            return await Context.DeckCards.Where(c => c.SessionId == sessionId).OrderBy(c => c.Order).FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
