using Entities.Common;
using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Avalon
{
    public class GameRole : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public GameRoleEnum RoleEnum { get; set; }
    }
}
