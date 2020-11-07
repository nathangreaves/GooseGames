using Entities.Fuji;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Fuji
{
    public class GameRepository : CommonRepository<Game>, IGameRepository
    {
        public FujiContext Context
        {
            get
            {
                return (FujiContext)DbContext;
            }
        }

        public GameRepository(FujiContext dbContext)
            : base(dbContext)
        {
        }
    }
}
