using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.JustOne.PlayerDetails
{
    public class PlayerDetailsResponse
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public int PlayerNumber { get; set; }
        public bool IsSessionMaster { get; set; }
    }
}
