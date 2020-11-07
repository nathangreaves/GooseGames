using Entities.Werewords;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords.PlayerStatus
{
    public interface IPlayerStatusKeyedService
    {
        Guid PlayerStatus { get; }
        Task<GenericResponse<string>> TransitionPlayerStatus(Guid roundId, PlayerRoundInformation playerRoundInformation);
    }
}
