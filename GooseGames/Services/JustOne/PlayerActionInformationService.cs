using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Microsoft.AspNetCore.SignalR;
using Models.Requests;
using Models.Responses;
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
        private readonly PlayerService _playerService;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly RequestLogger<PlayerActionInformationService> _logger;

        public PlayerActionInformationService(RoundService roundService, 
            Global.PlayerService playerService,
            IPlayerStatusRepository playerStatusRepository,
            IResponseRepository responseRepository, 
            RequestLogger<PlayerActionInformationService> logger)
        {
            _roundService = roundService;
            _playerService = playerService;
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
            var playersExceptActivePlayer = (await _playerService.GetForSessionAsync(request.SessionId)).Where(p => p.Id != currentRound.ActivePlayerId.Value);
            var playerIds = playersExceptActivePlayer.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting responses");
            var responses = await _responseRepository.FilterAsync(p => playerIds.Contains(p.PlayerId) && p.RoundId == currentRound.Id);

            _logger.LogTrace("Preparing result");
            var result = playersExceptActivePlayer.OrderBy(p => p.PlayerNumber).Select(p => new PlayerActionResponse
            {
                Id = p.Id,
                PlayerName = p.Name,
                PlayerNumber = p.PlayerNumber,
                Emoji = p.Emoji,
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

        private async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerInfoForPlayerStatus(PlayerSessionRequest request, Guid playerStatus, bool excludeActivePlayer = true)
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
            //var playersToSelect = await _playerRepository.FilterAsync(p => p.SessionId == request.SessionId && (!excludeActivePlayer || p.Id != currentRound.ActivePlayerId));
            //var playerIds = playersToSelect.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersThatHaveVoted = await _playerStatusRepository.FilterAsync(p => p.GameId == currentRound.GameId && (!excludeActivePlayer || p.PlayerId != currentRound.ActivePlayerId) && p.Status == playerStatus);

            var players = (await _playerService.GetForSessionAsync(request.SessionId)).Where(p => !excludeActivePlayer || p.Id != currentRound.ActivePlayerId);

            _logger.LogTrace("Preparing result");
            var result = players.Select(p => new PlayerActionResponse
            {
                Id = p.Id,
                PlayerName = p.Name,
                PlayerNumber = p.PlayerNumber,
                Emoji = p.Emoji,
                HasTakenAction = playersThatHaveVoted.Any(x => x.PlayerId == p.Id)
            });

            return GenericResponse<IEnumerable<PlayerActionResponse>>.Ok(result);
        }

        internal async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayersWaitingForRoundAsync(PlayerSessionRequest request)
        {
            return await GetPlayerInfoForPlayerStatus(request, PlayerStatusEnum.RoundWaiting, false);
        }
    }
}
