using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Avalon
{
    public class Game : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public Guid GodPlayerId { get; set; }
        public int NumberOfPlayers { get; set; }
        public ICollection<GameRole> Roles { get; set; }
        public ICollection<PlayerState> PlayerStates { get; set; }
        public ICollection<PlayerIntel> PlayerIntel { get; set; }
    }
}
