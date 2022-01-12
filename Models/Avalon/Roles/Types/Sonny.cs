using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Sonny : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Sonny;
        public override bool ViableForDrunkToMimic => true;

        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var goodPlayer = GetRandomEvilPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);
            var otherPlayer = GetRandomPlayerExcept(new List<Guid> { currentPlayer.PlayerId, goodPlayer.PlayerId }, players);

            var list = new List<Player>
            {
                goodPlayer,
                otherPlayer
            }.OrderBy(x => s_Random.Next());

            return list.Select(x => new PlayerIntel
            {
                IntelType = IntelTypeEnum.ContextDependant,
                IntelPlayerId = x.PlayerId,
                PlayerId = currentPlayer.PlayerId
            }).ToList();
        }

        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var player1 = GetRandomPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);
            var list = new List<Player>
            {
                player1,
                GetRandomPlayerExcept(new List<Guid> { currentPlayer.PlayerId, player1.PlayerId }, players)
            };

            return list
                .Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.ContextDependant))
                .ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 1;
        }
    }
}
