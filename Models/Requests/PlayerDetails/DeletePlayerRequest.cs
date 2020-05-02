using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.PlayerDetails
{
    public class DeletePlayerRequest
    {
        public Guid SessionMasterId { get; set; }
        public Guid PlayerToDeleteId { get; set; }
    }
}
