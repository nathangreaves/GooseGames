using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Codenames
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public IEnumerable<CodenamesWord> Words { get; set; }
        public int BlueWordsRemaining { get; set; }
        public int RedWordsRemaining { get; set; }
        public bool GameOver { get; set; }
        public bool BlueVictory { get; set; }
        public bool BlueTurn { get; set; }
    }
}
