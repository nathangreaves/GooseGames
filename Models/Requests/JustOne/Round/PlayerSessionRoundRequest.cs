using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.JustOne.Round
{
    public class PlayerSessionRoundRequest : PlayerSessionRequest
    {
        public Guid RoundId { get; set; }
    }
}
