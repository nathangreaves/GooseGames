using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Codenames
{
    public class RefreshSessionRequest
    {
        public Guid SessionId { get; set; }
    }
}
