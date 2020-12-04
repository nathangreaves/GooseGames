using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class ClueVoteResponse
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ClueId { get; set; }
    }

    public class ProposedClueResponse
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public int NumberOfLetters { get; set; }
        public int NumberOfPlayerLetters { get; set; }
        public int NumberOfNonPlayerLetters { get; set; }
        public bool WildcardUsed { get; set; }
        public int NumberOfBonusLetters { get; set; }
        public IEnumerable<ClueVoteResponse> Votes { get; set; }
        public bool VoteSuccess { get; set; }
    }

    public class ProposedCluesResponse
    {
        public IEnumerable<ProposedClueResponse> Clues { get; set; }

        public RoundStatusEnum RoundStatus { get; set; }
    }

    public enum RoundStatusEnum
    {
        ProposingClues = 1,
        ReceivedClue = 2
    }
}
