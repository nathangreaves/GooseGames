using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.PlayerDetails
{
    public class GetPlayerDetailsResponse
    {
        public bool SessionMaster { get; set; }
        public string SessionMasterName { get; set; }
        public int? SessionMasterPlayerNumber { get; set; }
        public string SessionMasterEmoji { get; set; }
        public string Password { get; set; }
        public string RandomEmoji { get; set; }
        public IEnumerable<PlayerDetailsResponse> Players { get; set; }
    }
}
