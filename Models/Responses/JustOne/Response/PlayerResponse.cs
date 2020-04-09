using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.JustOne.Response
{
    public class PlayerResponse
    {
        public Guid ResponseId { get; set; }
        public Guid PlayerId { get; set; }
        public int PlayerNumber { get; set; }
        public string PlayerName { get; set; }
        public string Response { get; set; }
        public bool ResponseInvalid { get; set; }
    }
}
