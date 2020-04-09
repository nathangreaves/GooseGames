using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.JustOne
{
    public class PlayerSessionRequest
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
