using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class GoodRoleBase : AvalonRoleBase
    {
        public override bool AppearsEvilToMerlin => false;
        public override bool AppearsEvilToEvil => false;
        public override bool ViableForMyopiaInfo => false;
    }
}
