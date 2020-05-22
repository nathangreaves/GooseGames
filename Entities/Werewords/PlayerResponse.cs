using Entities.Common;
using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords
{
    public class PlayerResponse : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid PlayerRoundInformationId { get; set; }
        public PlayerRoundInformation PlayerRoundInformation { get; set; }
        public PlayerResponseTypeEnum ResponseType { get; set; }
    }
}
