using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne.Enums
{
    public enum RoundStatusEnum
    {
        New = 0,
        WaitingForResponses = 1,
        WaitingForVotesOnDuplicates = 2,
        WaitingForLeaderResponse = 3,
        WaitingForVotesOnLeaderResponse = 5,
        LeaderResponseResolved = 6
    }
}
