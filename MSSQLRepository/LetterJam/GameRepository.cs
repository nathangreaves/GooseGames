using Entities.LetterJam;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.LetterJam
{
    public class GameRepository : CommonRepository<Game>, IGameRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public GameRepository(LetterJamContext context) 
            : base(context)
        {

        }
    }
}
