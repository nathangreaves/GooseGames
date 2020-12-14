using Entities.LetterJam;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.LetterJam
{
    public class FinalWordLetterRepository : CommonRepository<FinalWordLetter>, IFinalWordLetterRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public FinalWordLetterRepository(LetterJamContext context) 
            : base(context)
        {

        }
    }
}
