﻿using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class LoyalServantOfArthur : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.LoyalServantOfArthur;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 0;
        }
    }
}
