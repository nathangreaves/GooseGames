using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.JustOne.PlayerDetails
{
    public class DeletePlayerRequest
    {
        public Guid SessionMasterId { get; set; }
        public Guid PlayerToDeleteId { get; set; }
    }
}
