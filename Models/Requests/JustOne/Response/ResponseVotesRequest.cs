using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.JustOne.Response
{
    public class ResponseVotesRequest : PlayerSessionRequest
    {
        public IEnumerable<Guid> ValidResponses { get; set; }
    }
}
