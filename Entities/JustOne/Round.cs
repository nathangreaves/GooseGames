using Entities.Common;
using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne
{
    public class Round : IHasGuidId
    {
        public Round()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid? ActivePlayerId { get; set; }
        public Player ActivePlayer { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
        public string WordToGuess { get; set; }
        public RoundStatusEnum Status { get; set; }
        public RoundOutcomeEnum Outcome { get; set; }
        public ICollection<Response> Responses { get; set; }
    }
}
