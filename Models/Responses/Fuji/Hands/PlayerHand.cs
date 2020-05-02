using Models.Responses.Fuji.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Responses.Fuji.Hands
{
    public class PlayerHand : IHand
    {
        public IEnumerable<Card> Cards { get; set; }

        public int NumberOfCards => Cards.Count();
    }
}
