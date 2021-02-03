using Enums.Avalon;
using Models.Avalon.Roles.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles
{
    public static class AvalonRoleKey
    {
        private static readonly Dictionary<GameRoleEnum, AvalonRoleBase> s_Instances = new Dictionary<GameRoleEnum, AvalonRoleBase>
        {
            { GameRoleEnum.Merlin, new Merlin() },
            { GameRoleEnum.Percival, new Percival() },
            { GameRoleEnum.Oberon, new Oberon() },
            { GameRoleEnum.Morgana, new Morgana() },
            { GameRoleEnum.Mordred, new Mordred() },
            { GameRoleEnum.Assassin, new Assassin() },
            { GameRoleEnum.LoyalServantOfArthur, new LoyalServantOfArthur() },
            { GameRoleEnum.MinionOfMordred, new MinionOfMordred() }
        };

        public static AvalonRoleBase GetRole(GameRoleEnum x)
        {
            return s_Instances[x];
        }
        public static AvalonRoleBase TryGetRole(GameRoleEnum x)
        {
            try
            {
                return GetRole(x);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
