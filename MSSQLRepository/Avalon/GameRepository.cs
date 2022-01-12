using Entities.Avalon;
using MSSQLRepository.Contexts;
using RepositoryInterface.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Avalon
{
    public class GameRepository : CommonRepository<Game>, IGameRepository
    {
        public GameRepository(AvalonContext context) : base(context)
        {
        }
    }
}
