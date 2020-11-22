using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class ClueRequest : RoundRequest
    {
        public Guid? ClueId { get; set; }
    }
}
