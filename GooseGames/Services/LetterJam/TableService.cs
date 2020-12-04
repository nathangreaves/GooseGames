using Entities.LetterJam;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Requests;
using Models.Responses;
using Models.Responses.LetterJam;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class TableService
    {
        private readonly PlayerService _playerService;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly INonPlayerCharacterRepository _nonPlayerCharacterRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly RequestLogger<TableService> _logger;

        public TableService(
            PlayerService playerService,
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            INonPlayerCharacterRepository nonPlayerCharacterRepository,
            IRoundRepository roundRepository,
            RequestLogger<TableService> logger)
        {
            _playerService = playerService;
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _nonPlayerCharacterRepository = nonPlayerCharacterRepository;
            _roundRepository = roundRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<RoundResponse>> GetCurrentRoundAsync(PlayerSessionGameRequest request)
        {
            var game = await _gameRepository.GetAsync(request.GameId);
            if (!game.CurrentRoundId.HasValue)
            {
                return GenericResponse<RoundResponse>.Error("Unable to find current round");
            }

            var round = await _roundRepository.GetAsync(game.CurrentRoundId.Value);

            return GenericResponse<RoundResponse>.Ok(new RoundResponse 
            { 
                RoundId = round.Id,
                RoundStatus = (RoundStatusEnum)(int)round.RoundStatus
            });
        }

        internal async Task<GenericResponse<TableResponse>> GetTableAsync(PlayerSessionGameRequest request)
        {
            var game = await _gameRepository.GetAsync(request.GameId);
            if (game == null)
            {
                return GenericResponse<TableResponse>.Error("Could not find game");
            }
            if (!game.CurrentRoundId.HasValue)
            {
                return GenericResponse<TableResponse>.Error("Could not find current round");
            }

            var playerStates = await _playerStateRepository.FilterAsync(p => p.GameId == request.GameId);

            var numberOfPlayers = playerStates.Count;
            var gameConfig = GameConfigurationService.GetForPlayerCount(numberOfPlayers);
            var numberOfRedCluesPerPlayer = gameConfig.NumberOfRedCluesPerPlayer;

            List<NonPlayerCharacter> nonPlayerCharacters = new List<NonPlayerCharacter>();
            if (gameConfig.NonPlayerCharacters.Count > 0)
            {
                nonPlayerCharacters = await _nonPlayerCharacterRepository.FilterAsync(nPC => nPC.GameId == request.GameId);
            }
            var playerNumbers = await _playerService.GetPlayerNumbersAsync(playerStates.Select(p => p.PlayerId));

            return GenericResponse<TableResponse>.Ok(new TableResponse
            {
                CurrentRoundId = game.CurrentRoundId.Value,
                GreenCluesRemaining = game.GreenCluesRemaining,
                RedCluesRemaining = game.RedCluesRemaining,
                LockedCluesRemaining = game.LockedCluesRemaining,
                Players = new List<TablePlayerResponse>(playerStates.OrderBy(p => playerNumbers[p.PlayerId]).Select(p => 
                {
                    return new TablePlayerResponse
                    {
                        PlayerId = p.PlayerId,
                        NumberOfRedCluesGiven = (p.NumberOfCluesGiven > numberOfRedCluesPerPlayer) ? numberOfRedCluesPerPlayer : p.NumberOfCluesGiven,
                        NumberOfGreenCluesGiven = (p.NumberOfCluesGiven > numberOfRedCluesPerPlayer) ? p.NumberOfCluesGiven - numberOfRedCluesPerPlayer : 0,
                        NumberOfLetters = p.OriginalWordLength,
                        CurrentLetterIndex = p.CurrentLetterIndex,
                        CurrentLetterId = p.CurrentLetterId.Value
                    };
                })),
                NonPlayerCharacters = new List<TableNonPlayerCharacterResponse>(nonPlayerCharacters.OrderBy(nPC => nPC.PlayerNumber).Select(p =>
                {
                    return new TableNonPlayerCharacterResponse
                    {
                        NonPlayerCharacterId = p.Id,
                        CurrentLetterId = p.CurrentLetterId.Value,
                        NumberOfLettersRemaining = p.NumberOfLettersRemaining,
                        ClueReleased = p.ClueReleased
                    };
                }))
            });
        }
    }
}
