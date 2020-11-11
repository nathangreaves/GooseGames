using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class PlayerActionResponse
    {
        public Guid PlayerId { get; set; }
        public bool HasTakenAction { get; set; }
    }
}
