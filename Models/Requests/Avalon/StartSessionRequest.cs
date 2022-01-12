using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Avalon
{
    public class StartSessionRequest : PlayerSessionRequest
    {
        public StartSessionRequest()
        {
            GodMode = true;
        }
        public bool GodMode { get; set; }
        public IEnumerable<GameRoleEnum> Roles { get; set; }
    }
}
