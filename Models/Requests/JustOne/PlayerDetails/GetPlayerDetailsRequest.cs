using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.JustOne.PlayerDetails
{
    public class GetPlayerDetailsRequest
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
