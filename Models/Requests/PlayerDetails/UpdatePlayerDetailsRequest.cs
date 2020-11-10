using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.PlayerDetails
{
    public class UpdatePlayerDetailsRequest : PlayerSessionRequest
    {
        public string PlayerName { get; set; }
        public string Emoji { get; set; }

    }
}
