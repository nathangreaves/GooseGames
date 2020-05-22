using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords.Player
{
    public class PlayerResponse
    {
        public PlayerResponseType ResponseType { get; set; }
        public Guid PlayerId { get; set; }
    }
}
