using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.JustOne.Response
{
    public class PlayerResponses
    {
        public int ActivePlayerNumber { get; set; }
        public string ActivePlayerName { get; set; }
        public string ActivePlayerEmoji { get; set; }
        public string WordToGuess { get; set; }
        public IEnumerable<PlayerResponse> Responses { get; set; }
    }
}
