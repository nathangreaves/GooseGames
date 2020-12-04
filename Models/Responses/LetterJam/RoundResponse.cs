using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class RoundResponse
    {
        public Guid RoundId { get; set; }
        public RoundStatusEnum RoundStatus { get; set; }
    }
}
