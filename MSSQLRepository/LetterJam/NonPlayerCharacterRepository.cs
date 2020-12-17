using Entities.LetterJam;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.LetterJam
{
    public class NonPlayerCharacterRepository : CommonRepository<NonPlayerCharacter>, INonPlayerCharacterRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public NonPlayerCharacterRepository(LetterJamContext context)
            : base(context)
        {

        }
    }
}
