using Entities.Werewords;
using MSSQLRepository.Contexts;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Werewords
{
    public class RoundRepository : CommonRepository<Round>, IRoundRepository
    {
        private WerewordsContext Context => (WerewordsContext)DbContext;

        public RoundRepository(WerewordsContext context) : base(context)
        {

        }
    }
}
