using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PlayerActionInformationService
    {
        private readonly RoundService _roundService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly RequestLogger<PlayerActionInformationService> _logger;

        public PlayerActionInformationService(RoundService roundService, 
            IPlayerRepository playerRepository, 
            IPlayerStatusRepository playerStatusRepository,
            IResponseRepository responseRepository, 
            RequestLogger<PlayerActionInformationService> logger)
        {
            _roundService = roundService;
            _playerRepository = playerRepository;
            _playerStatusRepository = playerStatusRepository;
            _responseRepository = responseRepository;
            _logger = logger;
        }
        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseInfoAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Getting player response information", request);

            _logger.LogTrace("Getting current round for session");

            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (currentRound == null)
            {
                _logger.LogError("Unable to find single active round", request);

                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error("Failed to find round");
            }

            _logger.LogTrace("Getting players for session");
            var playersExceptActivePlayer = await _playerRepository.FilterAsync(p => p.SessionId == request.SessionId && p.Id != currentRound.ActivePlayerId);
            var playerIds = playersExceptActivePlayer.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting responses");
            var responses = await _responseRepository.FilterAsync(p => playerIds.Contains(p.PlayerId) && p.RoundId == currentRound.Id);

            _logger.LogTrace("Preparing result");
            var result = playersExceptActivePlayer.OrderBy(p => p.PlayerNumber).Select(p => new PlayerActionResponse
            {
                Id = p.Id,
                PlayerName = p.Name,
                PlayerNumber = p.PlayerNumber,
                HasTakenAction = responses.Any(x => x.PlayerId == p.Id)
            });

            return GenericResponse<IEnumerable<PlayerActionResponse>>.Ok(result);
        }

        internal async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseVoteInfoAsync(PlayerSessionRequest request)
        {
            return await GetPlayerInfoForPlayerStatus(request, PlayerStatusEnum.PassivePlayerWaitingForClueVotes);
        }

        internal async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseOutcomeVoteInfoAsync(PlayerSessionRequest request)
        {
            return await GetPlayerInfoForPlayerStatus(request, PlayerStatusEnum.PassivePlayerWaitingForOutcomeVotes);
        }

        private async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerInfoForPlayerStatus(PlayerSessionRequest request, Guid playerStatus)
        {
            _logger.LogTrace($"Getting player info for status {PlayerStatusEnum.GetDescription(playerStatus)}", request);

            _logger.LogTrace("Getting current round for session");

            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (currentRound == null)
            {
                _logger.LogError("Unable to find single active round", request);

                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error("Failed to find round");
            }

            _logger.LogTrace("Getting players for session");
            var playersExceptActivePlayer = await _playerRepository.FilterAsync(p => p.SessionId == request.SessionId && p.Id != currentRound.ActivePlayerId);
            var playerIds = playersExceptActivePlayer.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersThatHaveVoted = await _playerStatusRepository.FilterAsync(p => playerIds.Contains(p.PlayerId) && p.Status == playerStatus);

            _logger.LogTrace("Preparing result");
            var result = playersExceptActivePlayer.Select(p => new PlayerActionResponse
            {
                Id = p.Id,
                PlayerName = p.Name,
                PlayerNumber = p.PlayerNumber,
                HasTakenAction = playersThatHaveVoted.Any(x => x.PlayerId == p.Id)
            });

            return GenericResponse<IEnumerable<PlayerActionResponse>>.Ok(result);
        }
    }
}
