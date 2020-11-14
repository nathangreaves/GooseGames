using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class LetterCardsRequest : PlayerSessionGameRequest
    {
        public IEnumerable<Guid> CardIds { get; set; }
    }
}
