using Entities.LetterJam;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.LetterJam
{
    public class RoundRepository : CommonRepository<Round>, IRoundRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public RoundRepository(LetterJamContext context)
            : base(context)
        {

        }
    }
}
