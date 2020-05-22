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
        protected readonly IPlayerRepository _playerRepository;

        public abstract Guid PlayerStatus { get; }

        protected PlayerStatusKeyedServiceBase(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<GenericResponse<string>> TransitionPlayerStatus(Session session, PlayerRoundInformation playerRoundInformation)
        {
            var nextStatus = await GetNextStatusAsync(session, playerRoundInformation);

            Player player = playerRoundInformation.Player;
            player.Status = nextStatus;

            await _playerRepository.UpdateAsync(player);

            await NotifyOtherPlayersAsync(player);

            if (await ShouldTransitionRoundAsync(session, playerRoundInformation))
            {
                await TransitionRoundAsync(session, playerRoundInformation);                
            }

            return GenericResponse<string>.Ok(PlayerStatusEnum.GetDescription(playerRoundInformation.Player.Status));
        }

        internal abstract Task TransitionRoundAsync(Session session, PlayerRoundInformation playerRoundInformation);
        internal abstract Task<bool> ShouldTransitionRoundAsync(Session session, PlayerRoundInformation playerRoundInformation);
        internal abstract Task NotifyOtherPlayersAsync(Player player);
        protected abstract Task<Guid> GetNextStatusAsync(Session session, PlayerRoundInformation playerRoundInformation);
    }
}
