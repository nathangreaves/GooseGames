using Entities.Common;
using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords
{
    public class PlayerRoundInformation : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public Guid RoundId { get; set; }
        public Round Round { get; set; }
        public SecretRolesEnum SecretRole { get; set; }
        public bool IsMayor { get; set; }
        public int Ticks { get; set; }
        public int Crosses { get; set; }
        public int QuestionMarks { get; set; }
        public int SoClose { get; set; }
        public int Correct { get; set; }
    }
}
