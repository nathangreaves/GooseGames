using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Avalon
{
    public class RoleResponse
    {
        public GameRoleEnum RoleEnum{ get; set; }
        public int RoleWeight { get; set; }
        public bool Good { get; set; }
    }
}
