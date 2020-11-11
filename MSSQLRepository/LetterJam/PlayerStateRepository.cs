using Entities.LetterJam;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.LetterJam
{
    public class PlayerStateRepository : CommonRepository<PlayerState>, IPlayerStateRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public PlayerStateRepository(LetterJamContext context)
            : base(context)
        {

        }
    }
}
