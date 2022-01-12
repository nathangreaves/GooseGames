using Entities.Avalon;
using MSSQLRepository.Contexts;
using RepositoryInterface.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Avalon
{
    public class GameRoleRepository : CommonRepository<GameRole>, IGameRoleRepository
    {
        public GameRoleRepository(AvalonContext context) : base(context)
        {
        }
    }
}
