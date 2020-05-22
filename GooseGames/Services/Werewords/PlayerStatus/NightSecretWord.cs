using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Hubs;
using Models.Responses;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords.PlayerStatus
{
    public class NightSecretWordService : PlayerStatusKeyedServiceBase
    {
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly WerewordsHubContext _werewordsHubContext;

        public override Guid PlayerStatus => PlayerStatusEnum.NightSecretWord;

        public NightSecretWordService(IPlayerRepository playerRepository,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            IRoundRepository roundRepository,
            WerewordsHubContext werewordsHubContext) : base(playerRepository)
        {
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _roundRepository = roundRepository;
            _werewordsHubContext = werewordsHubContext;
        }

        protected override async Task<Guid> GetNextStatusAsync(Session session, PlayerRoundInformation playerRoundInformation)
        {
            return await Task.FromResult(PlayerStatusEnum.NightWaitingToWake);
        }

        internal override async Task NotifyOtherPlayersAsync(Player player)
        {
            await _werewordsHubContext.SendPlayerAwakeAsync(player.SessionId, new PlayerActionResponse 
            {
                PlayerName = player.Name,
                HasTakenAction = true,
                Id = player.Id,
                PlayerNumber = player.PlayerNumber
            });
        }
        internal override async Task<bool> ShouldTransitionRoundAsync(Session session, PlayerRoundInformation playerRoundInformation)
        {
            return await _playerRepository.CountAsync(p => p.SessionId == session.Id && p.Status != PlayerStatusEnum.NightWaitingToWake) == 0;
        }

        internal override async Task TransitionRoundAsync(Session session, PlayerRoundInformation playerRoundInformation)
        {
            var round = await _roundRepository.GetAsync(session.CurrentRoundId.Value);

            round.Status = RoundStatusEnum.Day;

            await _roundRepository.UpdateAsync(round);

            var playerInformations = await _playerRoundInformationRepository.GetForRoundAsync(round.Id);

            var mayor = playerInformations.Single(p => p.IsMayor);

            mayor.Player.Status = PlayerStatusEnum.DayMayor;

            var firstActivePlayer = FirstActivePlayer(playerInformations, mayor);

            foreach (var playerInformation in playerInformations)
            {
                var pI = playerInformation;
                if (pI.Id == playerRoundInformation.Id)
                {
                    pI = playerRoundInformation;
                }
                
                if (pI.IsMayor)
                {
                    pI.Player.Status = PlayerStatusEnum.DayMayor;
                }
                else if (pI.PlayerId == firstActivePlayer.Id)
                {
                    pI.Player.Status = PlayerStatusEnum.DayActive;
                }
                else
                {
                    pI.Player.Status = PlayerStatusEnum.DayPassive;
                }

                await _playerRoundInformationRepository.UpdateAsync(pI);
            }

            await _werewordsHubContext.SendDayBeginAsync(session.Id);
        }

        private Player FirstActivePlayer(IEnumerable<PlayerRoundInformation> playerInformations, PlayerRoundInformation mayor)
        {
            var orderedPlayerList = playerInformations.Select(p => p.Player).OrderBy(x => x.PlayerNumber).ToList();

            if (orderedPlayerList.Last().Id == mayor.PlayerId)
            {
                return orderedPlayerList.First();
            }
            var indexOfPrevious = orderedPlayerList.IndexOf(mayor.Player);
            return orderedPlayerList[indexOfPrevious + 1];
        }

    }
}
