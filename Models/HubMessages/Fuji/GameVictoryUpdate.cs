using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HubMessages.Fuji
{
    public class GameVictoryUpdate
    {
        public List<Guid> WinningPlayers { get; set; }
    }
}
