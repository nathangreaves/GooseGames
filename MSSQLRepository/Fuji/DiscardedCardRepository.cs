using Entities.Fuji.Cards;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Fuji
{
    public class DiscardedCardRepository : CommonRepository<DiscardedCard>, IDiscardedCardRepository
    {
        public DiscardedCardRepository(FujiContext dbContext) : base(dbContext)
        {
        }
    }
}
