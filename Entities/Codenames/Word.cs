using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Codenames
{
    public class CodenamesWord : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public Guid SessionWordsId { get; set; }
        public Session Session { get; set; }
        public string Word { get; set; }
        public WordTypeEnum WordType { get; set; }
        public bool Revealed { get; set; }
        public bool? RevealedByBlue { get; set; }
    }
}
