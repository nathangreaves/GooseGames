using Entities.Common;
using Entities.Fuji.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Fuji
{
    public class Game : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid? ActivePlayerId { get; set; }
        public ICollection<DeckCard> DeckCards { get; set; }
        public ICollection<DiscardedCard> DiscardedCards { get; set; }
    }
}
