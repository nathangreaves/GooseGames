using Entities.Werewords.Enums;
using Models.Requests;
using Models.Responses;
using Models.Responses.PlayerStatus;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords
{
    public class PlayerStatusService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ProgressRoundService _progressRoundService;

        public PlayerStatusService(IPlayerRepository playerRepository,
            ISessionRepository sessionRepository,
            ProgressRoundService progressRoundService)
        {
            _playerRepository = playerRepository;
            _sessionRepository = sessionRepository;
            _progressRoundService = progressRoundService;
        }

        internal async Task UpdateAllPlayersForSessionAsync(Guid sessionId, Guid playerStatus)
        {
            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId);

            foreach (var player in players)
            {
                player.Status = playerStatus;
            }

            await _playerRepository.UpdateRangeAsync(players);
        }

        internal async Task UpdatePlayerToStatusAsync(Guid playerId, Guid playerStatus)
        {
            var player = await _playerRepository.GetAsync(playerId);

            player.Status = playerStatus;

            await _playerRepository.UpdateAsync(player);
        }

        internal async Task UpdatePlayerToStatusAndProgressRoundAsync(PlayerSessionRequest request, Guid playerStatus)
        {
            await UpdatePlayerToStatusAsync(request.PlayerId, playerStatus);

            await _progressRoundService.ProgressRoundAsync(request.SessionId);
        }

        internal async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionRequest request, Guid status)
        {
            var player = await _playerRepository.SingleOrDefaultAsync(p => p.Id == request.PlayerId);
            if (player == null)
            {
                return NewResponse.Error<PlayerStatusValidationResponse>("You are not part of this session");
            }

            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {
                return NewResponse.Error<PlayerStatusValidationResponse>("You are not part of this session");
            }

            if (session.StatusId == SessionStatusEnum.Abandoned)
            {
                return NewResponse.Error<PlayerStatusValidationResponse>("This session was abandoned");
            }

            var response = new PlayerStatusValidationResponse
            {
                RequiredStatus = PlayerStatusEnum.GetDescription(player.Status),
                StatusCorrect = player.Status == status
            };
            return NewResponse.Ok(response);
        }
    }
}
