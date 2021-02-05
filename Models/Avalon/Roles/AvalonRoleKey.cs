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
            { GameRoleEnum.MinionOfMordred, new MinionOfMordred() },
            { GameRoleEnum.AssassinPlus, new AssassinPlus() },
            { GameRoleEnum.Coroner, new Coroner() },
            { GameRoleEnum.Witchdoctor, new Witchdoctor() },
            { GameRoleEnum.Instagawain, new Instagawain() },
            { GameRoleEnum.MerlinsApprentice, new MerlinsApprentice() },
            { GameRoleEnum.Yvain, new Yvain() },
            { GameRoleEnum.Cassia, new Cassia() },
            { GameRoleEnum.Guinevere, new Guinevere() },
            { GameRoleEnum.Arthur, new Arthur() },
            { GameRoleEnum.Gambler, new Gambler() },
            { GameRoleEnum.Macy, new Macy() },
            { GameRoleEnum.Sonny, new Sonny() },
            { GameRoleEnum.Matchmaker, new Matchmaker() },
            { GameRoleEnum.Cook, new Cook() },
            { GameRoleEnum.BraveSirRobin, new BraveSirRobin() },
            { GameRoleEnum.SirHector, new SirHector() },
            { GameRoleEnum.Visionary, new Visionary() },
            { GameRoleEnum.Myopia, new Myopia() },
            { GameRoleEnum.Drunk, new Drunk() },
            { GameRoleEnum.Sage, new Sage() },
            { GameRoleEnum.Karenevere, new Karenevere() },
            { GameRoleEnum.VoodooDoodooDoer, new VoodooDoodooDoer() },
            { GameRoleEnum.Armless, new Armless() },
            //{ GameRoleEnum.BlindAndArmless, new MinionOfMordred() },
            { GameRoleEnum.BraveSirRobin2, new BraveSirRobin2() }
        };

        public static AvalonRoleBase GetRole(GameRoleEnum x)
        {
            return s_Instances[x];
        }
    }
}
