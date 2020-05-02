using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HubMessages.Fuji
{
    public class PlayerDrawUpdate
    {
        public Guid PlayerId { get; set; }
        public Guid NewCardId { get; set; }
    }
}
