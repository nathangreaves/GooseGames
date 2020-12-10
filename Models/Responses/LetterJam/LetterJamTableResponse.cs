using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class TableResponse
    {
        public Guid CurrentRoundId { get; set; }
        public int RedCluesRemaining { get; set; }
        public int GreenCluesRemaining { get; set; }
        public int LockedCluesRemaining { get; set; }
        public List<TablePlayerResponse> Players { get; set; }
        public List<TableNonPlayerCharacterResponse> NonPlayerCharacters { get; set; }
    }

    public class TablePlayerResponse
    {
        public Guid PlayerId { get; set; }
        public int NumberOfRedCluesGiven { get; set; }
        public int NumberOfGreenCluesGiven { get; set; }
        public int NumberOfLetters { get; set; }
        public int? CurrentLetterIndex { get; set; }
        public Guid? CurrentLetterId { get; set; }
    }

    public class TableNonPlayerCharacterResponse
    {
        public Guid NonPlayerCharacterId { get; set; }
        public int NumberOfLettersRemaining { get; set; }
        public Guid CurrentLetterId { get; set; }
        public bool ClueReleased { get; set; }
    }
}
