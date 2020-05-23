using Models.Responses.Werewords.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords.Round
{
    public class RoundOutcomeResponse
    {
        public RoundOutcome RoundOutcome { get; set; }
        public string SecretWord { get; set; }
        public IEnumerable<RoundOutcomePlayerResponse> Players { get; set; }
    }
}
