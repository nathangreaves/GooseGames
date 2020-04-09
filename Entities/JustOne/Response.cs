using Entities.Common;
using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne
{
    public class Response : IHasGuidId
    {
        public Response()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Player Player { get; set; }
        public Guid PlayerId { get; set; }
        public Round Round { get; set; }
        public Guid RoundId { get; set; }
        public string Word { get; set; }
        public ResponseStatusEnum Status { get; set; }
    }
}
