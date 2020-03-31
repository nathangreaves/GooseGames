using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using GooseGames.Services.JustOne.RoundStatus;
using Models.Requests.JustOne;
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
        private readonly IRoundRepository _roundRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly PrepareNextRoundService _prepareNextRoundService;
        private readonly RoundServiceProvider _roundServiceProvider;
        private readonly RequestLogger<RoundService> _logger;

        private const int DefaultNumberOfRounds = 13;

        public RoundService(IRoundRepository roundRepository, 
            ISessionRepository sessionRepository, 
            IPlayerRepository playerRepository, 
            IResponseRepository responseRepository,
            PrepareNextRoundService prepareNextRoundService,
            RoundServiceProvider roundServiceProvider,
            RequestLogger<RoundService> logger)
        {
            _roundRepository = roundRepository;
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
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

            var activePlayer = await _playerRepository.GetAsync(round.ActivePlayerId.Value);

            _logger.LogTrace($"Got active player {activePlayer.Id} : {activePlayer.Name}", request);

            return GenericResponse<PassivePlayerRoundInformationResponse>.Ok(new PassivePlayerRoundInformationResponse 
            {
                ActivePlayerName = activePlayer.Name,
                ActivePlayerNumber = activePlayer.PlayerNumber,
                Word = round.WordToGuess
            });
        }

        internal async Task<GenericResponse<RoundOutcomeResponse>> GetRoundOutcomeAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace($"Getting round outcome", request);
            _logger.LogTrace($"Getting session");
            var session = await _sessionRepository.GetAsync(request.SessionId);

            _logger.LogTrace($"Getting round");
            var round = await _roundRepository.GetAsync(session.CurrentRoundId.Value);

            _logger.LogTrace($"Getting rounds remaining");
            var roundsRemaining = await _roundRepository.CountAsync(r => r.Status == RoundStatusEnum.New);

            _logger.LogTrace($"Getting active player");
            var activePlayer = await _playerRepository.GetAsync(round.ActivePlayerId.Value);

            _logger.LogTrace($"Getting active player response");
            var response = await _responseRepository.SingleOrDefaultAsync(r => r.PlayerId == activePlayer.Id && r.RoundId == round.Id);

            var outcome = new RoundOutcomeResponse
            {
                ActivePlayerName = activePlayer.Name,
                ActivePlayerNumber = activePlayer.PlayerNumber,
                GameEnded = roundsRemaining <= 0,
                RoundOutcome = (Models.Responses.JustOne.Round.RoundOutcomeEnum)(int)round.Outcome,
                Score = session.Score,
                WordGuessed = response != null ? response.Word.ToUpper() : null,
                WordToGuess = round.WordToGuess.ToUpper(),
                RoundId = round.Id
            };
            return GenericResponse<RoundOutcomeResponse>.Ok(outcome);
        }

        internal async Task PrepareRoundsAsync(Guid sessionId)
        {
            _logger.LogTrace("Preparing Round", sessionId);

            var session = await _sessionRepository.GetAsync(sessionId);

            _logger.LogTrace("Found session");

            var words = GetWords(session, DefaultNumberOfRounds);

            var rounds = words.Select(word => 
            {
                return new Round
                {
                    Session = session,
                    Status = RoundStatusEnum.New,
                    WordToGuess = word
                };
            });

            _logger.LogTrace("Inserting rounds");
            await _roundRepository.InsertRangeAsync(rounds);

            _logger.LogTrace("Preparing First round");
            var nextRound = await _prepareNextRoundService.PrepareNextRoundAsync(session.Id);

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

        public List<string> GetWords(Session session, int numberOfWords)
        {
            return TemporaryWordsList.GetWords(numberOfWords);
        }

        internal async Task<Round> GetCurrentRoundAsync(PlayerSessionRequest request)
        {
            return await _roundRepository.GetCurrentRoundForSessionAsync(request.SessionId);
        }
    }
}
