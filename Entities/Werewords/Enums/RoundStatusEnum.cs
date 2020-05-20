using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords.Enums
{
    public enum RoundStatusEnum
    {
        NightSecretRole = 1,
        NightChooseSecretWord = 2,
        NightRevealSecretWord = 3,
        Day = 4,
        DayVoteWerewolves = 5,
        DayVoteSeer = 6,
        Complete = 7
    }
}
