using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Hubs;
using GooseGames.Services.Global;
using Models.Responses;
using MSSQLRepository.Werewords;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords.PlayerStatus
{
    public class NightSecretWordService : PlayerStatusKeyedServiceBase
    {
        private readonly Global.PlayerService _playerService;
        private readonly IRoundRepository _roundRepository;
        private readonly WerewordsHubContext _werewordsHubContext;

        public override Guid PlayerStatus => PlayerStatusEnum.NightSecretWord;

        public NightSecretWordService(Global.PlayerService playerService,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            IRoundRepository roundRepository,
            WerewordsHubContext werewordsHubContext) : base(playerRoundInformationRepository)
        {
            _playerService = playerService;
            _roundRepository = roundRepository;
            _werewordsHubContext = werewordsHubContext;
        }

        protected override async Task<Guid> GetNextStatusAsync(Guid roundId, PlayerRoundInformation playerRoundInformation)
        {
            return await Task.FromResult(PlayerStatusEnum.NightWaitingToWake);
        }

        internal override async Task NotifyOtherPlayersAsync(PlayerRoundInformation playerRoundInformation)
        {
            var player = await _playerService.GetAsync(playerRoundInformation.PlayerId);

            await _werewordsHubContext.SendPlayerAwakeAsync(player.SessionId.Value, new PlayerActionResponse 
            {
                PlayerName = player.Name,
                HasTakenAction = true,
                Id = player.Id,
                PlayerNumber = player.PlayerNumber
            });
        }
        internal override async Task<bool> ShouldTransitionRoundAsync(Guid roundId, PlayerRoundInformation playerRoundInformation)
        {
            return await _playerRoundInformationRepository.CountAsync(p => p.RoundId == roundId && p.Status != PlayerStatusEnum.NightWaitingToWake) == 0;
        }

        internal override async Task TransitionRoundAsync(Guid roundId, PlayerRoundInformation playerRoundInformation)
        {
            var round = await _roundRepository.GetAsync(roundId);

            round.Status = RoundStatusEnum.Day;

            await _roundRepository.UpdateAsync(round);

            var playerInformations = await _playerRoundInformationRepository.GetForRoundAsync(round.Id);

            var mayor = playerInformations.Single(p => p.IsMayor);

            mayor.Status = PlayerStatusEnum.DayMayor;

            var firstActivePlayer = await FirstActivePlayerAsync(round.SessionId, mayor);

            foreach (var playerInformation in playerInformations)
            {
                var pI = playerInformation;
                if (pI.Id == playerRoundInformation.Id)
                {
                    pI = playerRoundInformation;
                }
                
                if (pI.IsMayor)
                {
                    pI.Status = PlayerStatusEnum.DayMayor;
                }
                else if (pI.PlayerId == firstActivePlayer)
                {
                    pI.Status = PlayerStatusEnum.DayActive;
                }
                else
                {
                    pI.Status = PlayerStatusEnum.DayPassive;
                }

                await _playerRoundInformationRepository.UpdateAsync(pI);
            }

            await _werewordsHubContext.SendDayBeginAsync(round.SessionId);
        }

        private async Task<Guid> FirstActivePlayerAsync(Guid sessionId, PlayerRoundInformation mayor)
        {
            return await _playerService.GetNextActivePlayerAsync(sessionId, mayor.PlayerId);
        }

    }
}
