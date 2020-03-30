using Entities.JustOne;
using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne.RoundStatus
{
    public interface IRoundStatusKeyedService
    {
        RoundStatusEnum RoundStatus { get; }

        Task UpdatePlayerStatusAsync(Round round);
        Task TransitionRoundStatusAsync(Round round);
        Task ConditionallyTransitionRoundStatusAsync(Round round);
    }
}
