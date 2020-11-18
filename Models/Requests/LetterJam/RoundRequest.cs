using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class RoundRequest : PlayerSessionGameRequest
    {
        public Guid? RoundId { get; set; }
    }
}
