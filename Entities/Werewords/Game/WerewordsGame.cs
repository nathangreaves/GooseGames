using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords.Game
{
    public class WerewordsGame
    {
        public Guid Id { get; set; }

        public string Password { get; set; }

        public int NumberOfPlayers { get; set; }
    }
}
