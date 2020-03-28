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

        Task UpdatePlayerStatusAsync(Guid sessionId, Guid roundId);
        Task TransitionRoundStatusAsync(Guid roundId);
    }
}
