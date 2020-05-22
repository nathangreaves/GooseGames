using Entities.Common;
using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords
{
    public class Round : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
        public Guid? MayorId { get; set; }
        public Player Mayor { get; set; }
        public string SecretWord { get; set; }
        public DateTime RoundStartedUtc { get; set; }
        public int RoundDurationMinutes { get; set; }
        public RoundStatusEnum Status { get; set; }
        public RoundOutcomeEnum Outcome { get; set; }
        public DateTime VoteStartedUtc { get; set; }
        public int VoteDurationSeconds { get; set; }
    }
}
