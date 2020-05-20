using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Codenames
{
    public class Session : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Password { get; set; }
        public bool IsBlueTurn { get; set; }
        public bool? BlueVictory { get; set; }
        public Guid SessionWordsId { get; set; }
        public ICollection<CodenamesWord> Words { get; set; }
    }
}
