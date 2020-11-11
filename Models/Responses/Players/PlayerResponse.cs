using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Players
{
    public class PlayerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Emoji { get; set; }
        public int PlayerNumber { get; set; }
    }
}
