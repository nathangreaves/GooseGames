using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Fuji
{
    public abstract class CardBase : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
        public int FaceValue { get; set; }
    }
}
