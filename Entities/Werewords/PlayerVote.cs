using Entities.Common;
using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords
{
    public class PlayerVote : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid PlayerId { get; set; }
        public Guid RoundId { get; set; }
        public Round Round { get; set; }
        public Guid VotedPlayerId { get; set; }
        public PlayerVoteTypeEnum VoteType { get; set; }
    }
}
