using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Werewords
{
    public class SubmitVoteRequest : PlayerSessionRequest
    {
        public Guid NominatedPlayerId { get; set; }
    }
}
