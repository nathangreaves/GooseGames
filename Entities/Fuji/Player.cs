using Entities.Common;
using Entities.Fuji.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Fuji
{
    public class Player : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
        public int PlayerNumber { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public ICollection<HandCard> Cards { get; set; }
        public Guid? PlayedCardId { get; set; }
        public HandCard PlayedCard { get; set; }
    }
}
