using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.JustOne.Round
{
    public class RoundOutcomeResponse
    {
        public bool GameEnded { get; set; }
        public int Score { get; set; }
        public string WordToGuess { get; set; }
        public string WordGuessed { get; set; }
        public string ActivePlayerName { get; set; }
        public int ActivePlayerNumber { get; set; }
        public string ActivePlayerEmoji { get; set; }
        public RoundOutcomeEnum RoundOutcome { get; set; }
        public Guid RoundId { get; set; }
        public RoundInformationResponse NextRoundInformation { get; set; }
    }
}
