using Models.Responses.Fuji.Cards;
using Models.Responses.Fuji.Hands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Fuji.Players
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public string Emoji { get; set; }
        public bool IsActivePlayer { get; set; }
        public IHand Hand { get; set; }
        public PlayedCard PlayedCard { get; set; }
    }
}
