using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Avalon
{
    public class RoleResponse
    {
        public GameRoleEnum RoleEnum { get; set; }
        public bool Good { get; set; }
        public bool ViableForDrunkToMimic { get; set; }
        public bool ViableForMyopiaInfo { get; set; }
    }
}
