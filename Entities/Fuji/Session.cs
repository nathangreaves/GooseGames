using Entities.Common;
using Entities.Fuji.Cards;
using Entities.Fuji.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Fuji
{
    public class Session : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Password { get; set; }
        public ICollection<Player> Players { get; set; }        
        public SessionStatusEnum StatusId { get; set; }        
        public Guid? SessionMasterId { get; set; }
        public Player SessionMaster { get; set; }
        public int Score { get; set; }
        public ICollection<DeckCard> DeckCards { get; set; }
        public ICollection<DiscardedCard> DiscardedCards { get; set; }
    }
}
