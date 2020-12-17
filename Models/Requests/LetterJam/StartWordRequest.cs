using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class StartWordRequest : PlayerSessionGameRequest
    {
        public string StartWord { get; set; }
        public Guid ForPlayerId { get; set; }
    }
}
