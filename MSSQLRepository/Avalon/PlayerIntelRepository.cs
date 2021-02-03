using Entities.Avalon;
using MSSQLRepository.Contexts;
using RepositoryInterface.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Avalon
{
    public class PlayerIntelRepository : CommonRepository<PlayerIntel>, IPlayerIntelRepository
    {
        public PlayerIntelRepository(AvalonContext context) : base(context)
        {
        }
    }
}
