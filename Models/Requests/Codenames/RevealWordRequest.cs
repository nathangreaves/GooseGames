using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Codenames
{
    public class RevealWordRequest
    {
        public Guid SessionId { get; set; }
        public Guid WordId { get; set; }
    }
}
