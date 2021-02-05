using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class BraveSirRobin2 : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.BraveSirRobin2;
        public override bool ViableForDrunkToMimic => true;
        public override bool ViableForMyopiaInfo => true;
        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var player = GetRandomSeenByMerlinAsEvilExcept(new List<Guid> { currentPlayer.PlayerId }, players);

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.AppearsEvil,
                    PlayerId = currentPlayer.PlayerId,
                    IntelPlayerId = player.PlayerId
                },
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.RoleSeesYouAsEvil,
                    PlayerId = player.PlayerId,
                    RoleKnowsYou = GameRoleEnum.BraveSirRobin2
                }
            };
        }

        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var list = new List<Player> 
            { 
                GetRandomPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players) 
            };
            return list
                .Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.AppearsEvil))
                .ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 1;
        }
    }
}
