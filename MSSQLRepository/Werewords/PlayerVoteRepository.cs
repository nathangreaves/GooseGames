using Entities.Werewords;
using MSSQLRepository.Contexts;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Werewords
{
    public class PlayerVoteRepository : CommonRepository<PlayerVote>, IPlayerVoteRepository
    {
        private WerewordsContext Context => (WerewordsContext)DbContext;

        public PlayerVoteRepository(WerewordsContext context) : base(context)
        {

        }
    }
}
