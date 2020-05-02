using Entities.Fuji.Cards;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Fuji
{
    public class DeckCardRepository : CommonRepository<DeckCard>, IDeckCardRepository
    {
        public DeckCardRepository(FujiContext dbContext) : base(dbContext)
        {
        }
    }
}
