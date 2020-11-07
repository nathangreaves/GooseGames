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
    public class NightRevealSecretRoleService : PlayerStatusKeyedServiceBase
    {
        private readonly IRoundRepository _roundRepository;

        public override Guid PlayerStatus => PlayerStatusEnum.NightRevealSecretRole;

        public NightRevealSecretRoleService(IRoundRepository roundRepository, IPlayerRoundInformationRepository playerRepository) : base(playerRepository)
        {
            _roundRepository = roundRepository;
        }

        protected override async Task<Guid> GetNextStatusAsync(Guid roundId, PlayerRoundInformation playerRoundInformation)
        {
            if (playerRoundInformation.IsMayor)
            {
                return PlayerStatusEnum.NightMayorPickSecretWord;
            }

            var currentRoundStatus = await _roundRepository.GetPropertyAsync(roundId, r => r.Status);

            if (currentRoundStatus == RoundStatusEnum.NightRevealSecretWord)
            {
                return PlayerStatusEnum.NightSecretWord;
            }

            return PlayerStatusEnum.NightWaitingForMayor;
        }

        internal override Task TransitionRoundAsync(Guid roundId, PlayerRoundInformation playerRoundInformation)
        {
            return Task.FromResult(false);
        }

        internal override Task<bool> ShouldTransitionRoundAsync(Guid roundId, PlayerRoundInformation playerRoundInformation)
        {
            return Task.FromResult(false);
        }

        internal override Task NotifyOtherPlayersAsync(PlayerRoundInformation playerRoundInformation)
        {
            return Task.FromResult(false);
        }
    }
}
