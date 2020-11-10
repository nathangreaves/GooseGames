using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using GooseGames.Services.Global;
using GooseGames.Services.JustOne.RoundStatus;
using Models.Enums;
using Models.Requests;
using Models.Requests.JustOne.Round;
using Models.Responses;
using Models.Responses.JustOne.Round;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class RoundService
    {
        private readonly Global.SessionService _sessionService;
        private readonly PlayerService _playerService;
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly PrepareNextRoundService _prepareNextRoundService;
        private readonly RoundServiceProvider _roundServiceProvider;
        private readonly RequestLogger<RoundService> _logger;

        private const int DefaultNumberOfRounds = 13;

        public RoundService(Global.SessionService sessionService,
            Global.PlayerService playerService,         
            IGameRepository gameRepository,
            IRoundRepository roundRepository, 
            IResponseRepository responseRepository,
            PrepareNextRoundService prepareNextRoundService,
            RoundServiceProvider roundServiceProvider,
            RequestLogger<RoundService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _gameRepository = gameRepository;
            _roundRepository = roundRepository;
            _responseRepository = responseRepository;
            _prepareNextRoundService = prepareNextRoundService;
            _roundServiceProvider = roundServiceProvider;
            _logger = logger;
        }

        internal async Task<GenericResponse<PassivePlayerRoundInformationResponse>> GetPassivePlayerRoundInfoAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Getting passive player round info", request);

            var round = await GetCurrentRoundAsync(request);

            _logger.LogTrace($"Got round {round.Id} : {round.WordToGuess}", request);

            var activePlayer = await _playerService.GetAsync(round.ActivePlayerId.Value);

            _logger.LogTrace($"Got active player {activePlayer.Id} : {activePlayer.Name}", request);

            return GenericResponse<PassivePlayerRoundInformationResponse>.Ok(new PassivePlayerRoundInformationResponse 
            {
                ActivePlayerName = activePlayer.Name,
                ActivePlayerNumber = activePlayer.PlayerNumber,
                ActivePlayerEmoji = activePlayer.Emoji,
                Word = round.WordToGuess
            });
        }

        internal async Task<GenericResponse<RoundOutcomeResponse>> GetRoundOutcomeAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace($"Getting round outcome", request);
            _logger.LogTrace($"Getting session");
            var gameId = await _sessionService.GetGameIdAsync(request.SessionId, Entities.Global.Enums.GameEnum.JustOne);
            if (!gameId.HasValue)
            {
                return GenericResponse<RoundOutcomeResponse>.Error("Unable to find game");
            }

            var game = await _gameRepository.GetAsync(gameId.Value);
            if (game == null)
            {
                return GenericResponse<RoundOutcomeResponse>.Error($"Unable to find game from gameId {gameId.Value}");
            }

            if (game.CurrentRoundId == null)
            {
                return GenericResponse<RoundOutcomeResponse>.Error($"Unable to find current round for gameId {gameId.Value}");
            }

            _logger.LogTrace($"Getting round");
            var round = await _roundRepository.GetAsync(game.CurrentRoundId.Value);

            _logger.LogTrace($"Getting rounds remaining");
            var roundsRemaining = await _roundRepository.CountAsync(r => r.Status == RoundStatusEnum.New && r.GameId == gameId.Value);

            _logger.LogTrace($"Getting active player");
            var activePlayer = await _playerService.GetAsync(round.ActivePlayerId.Value);

            _logger.LogTrace($"Getting active player response");
            var response = await _responseRepository.SingleOrDefaultAsync(r => r.PlayerId == activePlayer.Id && r.RoundId == round.Id);

            bool gameEnded = roundsRemaining <= 0;

             _logger.LogTrace("Getting total number of rounds");
            var roundsTotal = await _roundRepository.CountAsync(r => r.GameId == gameId.Value);

            var nextRoundInformation = new RoundInformationResponse
            {
                Score = game.Score,
                RoundsTotal = roundsTotal,
                RoundNumber = gameEnded ? roundsTotal : (roundsTotal - roundsRemaining) + 1
            };

            var outcome = new RoundOutcomeResponse
            {
                ActivePlayerName = activePlayer.Name,
                ActivePlayerNumber = activePlayer.PlayerNumber,
                ActivePlayerEmoji = activePlayer.Emoji,
                GameEnded = gameEnded,
                RoundOutcome = (Models.Responses.JustOne.Round.RoundOutcomeEnum)(int)round.Outcome,
                Score = game.Score,
                WordGuessed = response != null ? response.Word.ToUpper() : null,
                WordToGuess = round.WordToGuess.ToUpper(),
                RoundId = round.Id,
                NextRoundInformation = nextRoundInformation
            };
            return GenericResponse<RoundOutcomeResponse>.Ok(outcome);
        }

        internal async Task PrepareRoundsAsync(Guid gameId, IEnumerable<WordListEnum> includedWordLists)
        {
            _logger.LogTrace("Preparing Round", gameId);

            var game = await _gameRepository.GetAsync(gameId);

            _logger.LogTrace("Found session");

            var words = GetWords(DefaultNumberOfRounds, includedWordLists);

            var rounds = words.Select(word => 
            {
                return new Round
                {
                    GameId = gameId,
                    SessionId = game.SessionId,
                    Status = RoundStatusEnum.New,
                    WordToGuess = word
                };
            });

            _logger.LogTrace("Inserting rounds");
            await _roundRepository.InsertRangeAsync(rounds);

            _logger.LogTrace("Preparing First round");
            var nextRound = await _prepareNextRoundService.PrepareGameNextRoundAsync(gameId, game.SessionId);

            await ProgressRoundAsync(nextRound.Id);
        }

        internal async Task ProgressRoundAsync(Guid roundId)
        {
            var round = await _roundRepository.GetAsync(roundId);
            await ProgressRoundAsync(round);
;        }

        internal async Task ProgressRoundAsync(Round round)
        {
            var roundStatusService = _roundServiceProvider.GetService(round.Status);
            await roundStatusService.ConditionallyTransitionRoundStatusAsync(round);
        }

        public List<string> GetWords(int numberOfWords, IEnumerable<WordListEnum> includedWordLists)
        {
            return StaticWordsList.GetWords(numberOfWords, includedWordLists);
        }

        internal async Task<Round> GetCurrentRoundAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace($"Getting session");
            var gameId = await _sessionService.GetGameIdAsync(request.SessionId, Entities.Global.Enums.GameEnum.JustOne);
            if (!gameId.HasValue)
            {
                throw new NullReferenceException($"Game did not exist for session {request.SessionId}");
            }

            return await _roundRepository.GetCurrentRoundForGameAsync(gameId.Value);
        }
    }
}
