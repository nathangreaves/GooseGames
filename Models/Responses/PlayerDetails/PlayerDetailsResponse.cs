using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.PlayerDetails
{
    public class PlayerDetailsResponse
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public string Emoji { get; set; }
        public bool IsSessionMaster { get; set; }
        public bool Ready { get; set; }
    }
}
