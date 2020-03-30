using Entities.JustOne;
using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne.RoundStatus
{
    public delegate IRoundStatusKeyedService RoundStatusComponent(RoundStatusEnum roundStatus);

    public abstract class RoundStatusKeyedServiceBase : IRoundStatusKeyedService
    {
        public abstract RoundStatusEnum RoundStatus { get; }

        public abstract Task ConditionallyTransitionRoundStatusAsync(Round round);
    }
}
