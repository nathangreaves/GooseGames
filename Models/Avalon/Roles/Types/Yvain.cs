using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Yvain : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Yvain;

        public override bool AppearsEvilToMerlin => true;
        public override bool AppearsEvilToEvil => true;
        public override bool ViableForDrunkToMimic => false;

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            var assassinPlusInPlay = rolesInPlay.Any(x => x is AssassinPlus);

            short weight = 1; //Causes evil to potentially pass a mission they otherwise wouldn't

            if (assassinPlusInPlay)
            {
                if (numberOfPlayers < 7)
                {
                    weight -= 2; //Makes Assassin+ job easier as they will have probably sussed Yvain by the end of the game if good win.
                }
                else
                {
                    weight -= 1;
                }
            }

            return weight;
        }
    }
}
