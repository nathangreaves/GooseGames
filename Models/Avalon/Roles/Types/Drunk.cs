using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    //TODO: Need to add extra column to player state for Drunk Role.

    public class Drunk : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Drunk;

        public override bool ViableForDrunkToMimic => false;

        public override GameRoleEnum GetAssumedRole(IEnumerable<AvalonRoleBase> allRoles)
        {
            var playersForDrunkToMimic = allRoles.Where(x => x.ViableForDrunkToMimic).ToList();

            if (playersForDrunkToMimic.Any())
            {
                return playersForDrunkToMimic[s_Random.Next(0, playersForDrunkToMimic.Count)].RoleEnum;
            }

            return base.GetAssumedRole(allRoles);
        }

        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var assumedRole = currentPlayer.AssumedRole;

            return assumedRole.GenerateDrunkIntel(currentPlayer, players, allRoles);
        }

        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return new List<PlayerIntel>();
        }

        public override short GetRoleWeightInPlayAgnostic(int numberOfPlayers, int numberOfGoodPlayers, IEnumerable<AvalonRoleBase> allRoles)
        {
            var numberOfDrunkTargets = allRoles.Count(x => x.ViableForDrunkToMimic);

            var chanceOfDrunk = 1.0 / (numberOfDrunkTargets + 1);

            return (short)Math.Ceiling(-5 * chanceOfDrunk);
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            short weight = 0;
            if (allRoles.Any(x => x.ViableForDrunkToMimic))
            {
                weight -= 1;
            }
            if (allRoles.Any(x => x is Merlin))
            {
                weight -= 1;
            }
            return weight;
        }
    }
}
