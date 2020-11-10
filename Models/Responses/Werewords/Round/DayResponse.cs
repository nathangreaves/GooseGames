using Models.Enums.Werewords;
using Models.Responses.Werewords.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords.Round
{
    public class DayResponse
    {
        public Guid RoundId { get; set; }
        public Guid MayorPlayerId { get; set; }
        public string MayorName { get; set; }
        public string MayorEmoji { get; set; }
        public IEnumerable<PlayerRoundInformationResponse> Players { get; set; }
        public string SecretWord { get; set; }
        public bool IsActive { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? VoteEndTime { get; set; }
        public SecretRole SecretRole { get; set; }
        public bool SoCloseSpent { get; set; }
        public bool WayOffSpent { get; set; }
    }
}
