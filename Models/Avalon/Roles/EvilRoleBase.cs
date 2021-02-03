using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class EvilRoleBase : AvalonRoleBase
    {
        public override bool KnownToMerlin => true;
        public override bool KnownToEvil => true;

    }
}
