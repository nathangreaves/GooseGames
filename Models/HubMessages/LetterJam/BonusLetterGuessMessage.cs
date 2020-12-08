using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HubMessages.LetterJam
{
    public class BonusLetterGuessMessage
    {
        public Guid PlayerId { get; set; }
        public Guid CardId { get; set; }
        public char GuessedLetter { get; set; }
        public char ActualLetter { get; set; }
        public bool Correct { get; set; }
    }
}
