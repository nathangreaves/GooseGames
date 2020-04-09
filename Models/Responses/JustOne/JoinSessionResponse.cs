using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Responses.JustOne
{
    public class JoinSessionResponse
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
