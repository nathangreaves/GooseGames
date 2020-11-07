using Entities.Common;
using Entities.Fuji.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Fuji
{
    public class PlayerInformation : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        //public Guid SessionId { get; set; }
        //public Global.Session Session { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public Guid PlayerId { get; set; }
        public ICollection<HandCard> Cards { get; set; }
        public Guid? PlayedCardId { get; set; }
        public HandCard PlayedCard { get; set; }
    }
}
