using Models.Responses.Fuji.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Fuji
{
    public class SessionResponse
    {
        public IEnumerable<Player> Players { get; set; }
    }
}
