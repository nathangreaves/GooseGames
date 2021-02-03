using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Avalon
{
    public class StartSessionRequest : PlayerSessionRequest
    {
        public IEnumerable<GameRoleEnum> Roles { get; set; }
    }
}
