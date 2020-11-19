using Entities.LetterJam;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.LetterJam
{
    public class ClueLetterRepository : CommonRepository<ClueLetter>, IClueLetterRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public ClueLetterRepository(LetterJamContext context)
            : base(context)
        {

        }
    }
}
