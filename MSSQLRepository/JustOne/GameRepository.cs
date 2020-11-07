using Entities.Common;
using Entities.JustOne;
using Entities.JustOne.Enums;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.JustOne
{
    public class GameRepository : CommonRepository<Game>, IGameRepository
    {
        public JustOneContext Context
        {
            get
            {
                return (JustOneContext)DbContext;
            }
        }

        public GameRepository(JustOneContext dbContext) 
            : base(dbContext)
        {
        }

    }
}
