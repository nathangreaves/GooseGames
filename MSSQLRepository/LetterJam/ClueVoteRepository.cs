using Entities.LetterJam;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.LetterJam
{
    public class ClueVoteRepository : CommonRepository<ClueVote>, IClueVoteRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public ClueVoteRepository(LetterJamContext context)
            : base(context)
        {

        }
    }
}
