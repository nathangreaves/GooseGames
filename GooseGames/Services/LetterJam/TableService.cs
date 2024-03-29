﻿using Entities.LetterJam;
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
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly RequestLogger<TableService> _logger;

        public TableService(
            PlayerService playerService,
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            INonPlayerCharacterRepository nonPlayerCharacterRepository,
            IRoundRepository roundRepository,
            ILetterCardRepository letterCardRepository,
            RequestLogger<TableService> logger)
        {
            _playerService = playerService;
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _nonPlayerCharacterRepository = nonPlayerCharacterRepository;
            _roundRepository = roundRepository;
            _letterCardRepository = letterCardRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<RoundResponse>> GetCurrentRoundAsync(PlayerSessionGameRequest request)
        {
            var game = await _gameRepository.GetAsync(request.GameId);

            if (!(game.GameStatus == Entities.LetterJam.Enums.GameStatus.RevealingFinalWords || game.GameStatus == Entities.LetterJam.Enums.GameStatus.Finished)
                && !game.CurrentRoundId.HasValue)
            {
                return GenericResponse<RoundResponse>.Error("Unable to find current round");
            }
            Round round = null;
            RoundStatusEnum roundStatus = RoundStatusEnum.GameEnd;
            if (game.CurrentRoundId.HasValue)
            {
                round = await _roundRepository.GetAsync(game.CurrentRoundId.Value);
                roundStatus = (RoundStatusEnum)(int)round.RoundStatus;
            }
            var playerState = await _playerStateRepository.SingleOrDefaultAsync(s => s.PlayerId == request.PlayerId);

            return GenericResponse<RoundResponse>.Ok(new RoundResponse 
            { 
                RoundId = round?.Id,
                RoundStatus = roundStatus,
                PlayerStatus = playerState.StatusDescription
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

            var bonusCards = await _letterCardRepository
                .GetPropertyForFilterAsync(lC => 
                    lC.GameId == request.GameId && lC.BonusLetter && !lC.Discarded && lC.PlayerId == null,
                    lC => new KeyValuePair<Guid, Guid>(lC.Id, lC.Id)
                );

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
                        CurrentLetterId = p.CurrentLetterId
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
                })),
                BonusCardIds = bonusCards.Keys
            });
        }
    }
}
