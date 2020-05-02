using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Fuji.Cards
{
    public class Card : ICard
    {
        public int FaceValue { get; set; }
        public Guid Id { get; set; }
    }
}
