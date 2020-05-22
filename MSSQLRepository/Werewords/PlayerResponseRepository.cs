using Entities.Werewords;
using Microsoft.EntityFrameworkCore;
using MSSQLRepository.Contexts;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Werewords
{
    public class PlayerResponseRepository : CommonRepository<PlayerResponse>, IPlayerResponseRepository
    {
        private WerewordsContext Context => (WerewordsContext)DbContext;
        public PlayerResponseRepository(WerewordsContext dbContext) : base(dbContext)
        {
        }
    }
}
