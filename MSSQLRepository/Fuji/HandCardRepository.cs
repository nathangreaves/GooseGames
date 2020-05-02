using Entities.Fuji.Cards;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Fuji
{
    public class HandCardRepository : CommonRepository<HandCard>, IHandCardRepository
    {
        public HandCardRepository(FujiContext dbContext) : base(dbContext)
        {
        }
    }
}
