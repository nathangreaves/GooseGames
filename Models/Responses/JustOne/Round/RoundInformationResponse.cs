using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.JustOne.Round
{
    public class RoundInformationResponse
    {
        public int RoundNumber { get; set; }
        public int RoundsTotal { get; set; }
        public Guid RoundId { get; set; }
        public int Score { get; set; }
    }
}
