using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class ClueLetterRequest
    {
        public Guid? LetterId { get; set; }
        public bool IsWildCard { get; set; }
    }

    public class SubmitClueRequest : RoundRequest
    {
        public IEnumerable<ClueLetterRequest> ClueLetters { get; set; }
    }
}
