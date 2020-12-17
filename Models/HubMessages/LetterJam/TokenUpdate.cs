using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HubMessages.LetterJam
{
    public class TokenUpdate
    {
        public Guid? AddRedTokenToPlayerId { get; set; }
        public Guid? AddGreenTokenToPlayerId { get; set; }
        public IEnumerable<Guid> UnlockTokensFromNonPlayerCharacterIds { get; set; }
        public int? UnlockTokensFromSupply { get; set; }
    }
}
