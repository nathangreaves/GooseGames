using Entities.Werewords;
using Entities.Werewords.Enums;
using Models.Responses;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords.PlayerStatus
{
    public abstract class PlayerStatusKeyedServiceBase : IPlayerStatusKeyedService
    {
        protected readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;

        public abstract Guid PlayerStatus { get; }

        protected PlayerStatusKeyedServiceBase(IPlayerRoundInformationRepository playerRepository)
        {
            _playerRoundInformationRepository = playerRepository;
        }

        public async Task<GenericResponse<string>> TransitionPlayerStatus(Guid roundId, PlayerRoundInformation playerRoundInformation)
        {
            var nextStatus = await GetNextStatusAsync(roundId, playerRoundInformation);

            playerRoundInformation.Status = nextStatus;

            await _playerRoundInformationRepository.UpdateAsync(playerRoundInformation);

            await NotifyOtherPlayersAsync(playerRoundInformation);

            if (await ShouldTransitionRoundAsync(roundId, playerRoundInformation))
            {
                await TransitionRoundAsync(roundId, playerRoundInformation);                
            }

            return GenericResponse<string>.Ok(PlayerStatusEnum.GetDescription(playerRoundInformation.Status));
        }

        internal abstract Task TransitionRoundAsync(Guid roundId, PlayerRoundInformation playerRoundInformation);
        internal abstract Task<bool> ShouldTransitionRoundAsync(Guid roundId, PlayerRoundInformation playerRoundInformation);
        internal abstract Task NotifyOtherPlayersAsync(PlayerRoundInformation playerRoundInformation);
        protected abstract Task<Guid> GetNextStatusAsync(Guid roundId, PlayerRoundInformation playerRoundInformation);
    }
}
