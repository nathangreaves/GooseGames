using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    //TODO: Need to add extra column to player state for Drunk Role.

    //public class Drunk : GoodRoleBase
    //{
    //    public override GameRoleEnum RoleEnum => GameRoleEnum.Drunk;

    //    public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
    //    {
    //        var listOfViableDrunks = new List<GameRoleEnum> 
    //        {
    //            GameRoleEnum.Merlin,
    //            GameRoleEnum.MerlinsApprentice,
    //            GameRoleEnum.Macy,
    //            GameRoleEnum.Sonny,
    //            GameRoleEnum.Guinevere,
    //            GameRoleEnum.Visionary,
    //            GameRoleEnum.Myopia,
    //            GameRoleEnum.Matchmaker,
    //            GameRoleEnum.Cook
    //        };
    //        var playersForDrunkToMimic = players.Where(x => listOfViableDrunks.Contains(x.RoleEnum));

    //        if (!playersForDrunkToMimic.Any())
    //        {
    //            return new List<PlayerIntel> { 
    //                new PlayerIntel 
    //                {
    //                    IntelType = IntelTypeEnum.DrunkRole,
    //                    PlayerId = currentPlayerId
    //                } 
    //            };
    //        }

    //        var randomPlayerForDrunkToMimic = players.
    //    }

    //    public override short GetRoleWeight(int numberOfPlayers)
    //    {
    //        return -2;
    //    }
    //}
}
