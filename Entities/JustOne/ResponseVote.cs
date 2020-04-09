using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne
{
    public class ResponseVote : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public Guid ResponseId { get; set; }
        public Response Response { get; set; }
    }
}
