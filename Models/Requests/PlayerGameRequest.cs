using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests
{
    public class PlayerSessionGameRequest : PlayerSessionRequest
    {
        public Guid GameId { get; set; }
    }
}
