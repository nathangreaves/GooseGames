using Models.Responses.Werewords.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords.Round
{
    public class RoundResponse
    {
        public Guid Id { get; set; }
        public IEnumerable<PlayerRoundInformationResponse> Players { get; set; }
        public string SecretWord { get; set; }
    }
}
