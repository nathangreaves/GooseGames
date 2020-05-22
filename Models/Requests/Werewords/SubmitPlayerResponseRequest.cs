using Models.Responses.Werewords.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Werewords
{
    public class SubmitPlayerResponseRequest : PlayerSessionRequest
    {
        public PlayerResponseType ResponseType { get; set; }
        public Guid RespondingPlayerId { get; set; }
    }
}
