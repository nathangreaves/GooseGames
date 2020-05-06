using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HubMessages.Fuji
{
    public class FujiUpdate
    {
        public List<PlayedCardUpdate> PlayedCards { get; set; }
        public List<PlayerDiscardUpdate> DiscardedCards { get; set; }
        public List<PlayerDrawUpdate> NewDraws { get; set; }
        public ActivePlayerUpdate ActivePlayerUpdate { get; set; }
        public GameVictoryUpdate GameVictoryUpdate { get; set; }
    }
}
