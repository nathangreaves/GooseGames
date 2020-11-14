using Entities.LetterJam.Enums;
using GooseGames.Services.Global;
using Models.Requests;
using Models.Responses;
using Models.Responses.LetterJam;
using Models.Responses.PlayerStatus;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class PlayerStatusService

    {
        private readonly Global.SessionService _sessionService;
        private readonly GlobalPlayerStatusService _playerStatusService;
        private readonly IPlayerStateRepository _playerStateRepository;

        public PlayerStatusService(
            Global.SessionService sessionService,
            Global.GlobalPlayerStatusService playerStatusService,
            IPlayerStateRepository playerStateRepository)
        {
            _sessionService = sessionService;
            _playerStatusService = playerStatusService;
            _playerStateRepository = playerStateRepository;
        }

        internal async Task UpdateAllPlayersForGameAsync(Guid gameId, PlayerStatusId playerStatus)
        {
            var players = await _playerStateRepository.FilterAsync(p => p.GameId == gameId);

            foreach (var player in players)
            {
                player.Status = playerStatus;
            }

            await _playerStateRepository.UpdateRangeAsync(players);
        }


        internal async Task ConditionallyUpdateAllPlayersForSessionAsync(Guid gameId, PlayerStatusId fromStatus, PlayerStatusId toStatus)
        {
            var players = await _playerStateRepository.FilterAsync(p => p.GameId == gameId && p.Status == fromStatus);

            foreach (var player in players)
            {
                player.Status = toStatus;
            }

            await _playerStateRepository.UpdateRangeAsync(players);
        }

        internal async Task UpdatePlayerToStatusAsync(Guid playerId, Guid gameId, PlayerStatusId playerStatus)
        {
            var player = await _playerStateRepository.SingleOrDefaultAsync(p => p.PlayerId == playerId && p.GameId == gameId);

            player.Status = playerStatus;

            await _playerStateRepository.UpdateAsync(player);
        }

        internal async Task<bool> AllPlayersMatchStatusAsync(Guid gameId, PlayerStatusId playerStatusId)
        {
            var players = await _playerStateRepository.FilterAsync(p => p.GameId == gameId);

            return players.All(p => p.Status == playerStatusId);
        }

        internal async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionPossibleGameRequest request, PlayerStatusId status)
        {
            var globalResponse = await _playerStatusService.ValidatePlayerStatusAsync(request, Entities.Global.Enums.GameEnum.LetterJam);
            if (!globalResponse.Success)
            {
                return GenericResponse<LetterJamPlayerStatusValidationResponse>.Error(globalResponse.ErrorCode);
            }
            var globalPlayerStatus = globalResponse.Data;

            PlayerStatusId playerStatus;
            Guid? gameId = request.GameId;
            if (globalPlayerStatus == Entities.Global.Enums.PlayerStatusEnum.Lobby)
            {
                playerStatus = PlayerStatus.InLobby;
            }
            else if (globalPlayerStatus == Entities.Global.Enums.PlayerStatusEnum.Ready)
            {
                playerStatus = PlayerStatus.InLobby;
            }
            else
            {                
                if (!gameId.HasValue)
                {
                    gameId = await _sessionService.GetGameIdAsync(request.SessionId, Entities.Global.Enums.GameEnum.LetterJam);
                }
                if (gameId == null)
                {
                    return GenericResponse<LetterJamPlayerStatusValidationResponse>.Error("Could not find game");
                }
                var player = await _playerStateRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId && p.GameId == gameId.Value);
                if (player == null)
                {
                    return GenericResponse<LetterJamPlayerStatusValidationResponse>.Error("Could not find letter jam player");
                }

                playerStatus = player.Status;
            }

            var response = new LetterJamPlayerStatusValidationResponse
            {
                RequiredStatus = PlayerStatus.GetDescription(playerStatus),
                StatusCorrect = playerStatus == status,
                GameId = gameId
            };
            return GenericResponse<LetterJamPlayerStatusValidationResponse>.Ok(response);
        }

        internal async Task<GenericResponse<IEnumerable<Models.Responses.LetterJam.PlayerActionResponse>>> GetPlayerActionsAsync(PlayerSessionGameRequest request, PlayerStatusId desiredPlayerStatus)
        {
            var playerStates = await _playerStateRepository.FilterAsync(p => p.GameId == request.GameId);

            return GenericResponse<IEnumerable<Models.Responses.LetterJam.PlayerActionResponse>>.Ok(playerStates.Select(p => new Models.Responses.LetterJam.PlayerActionResponse
            {
                PlayerId = p.PlayerId,
                HasTakenAction = p.Status == desiredPlayerStatus
            }));
        }
    }
}
