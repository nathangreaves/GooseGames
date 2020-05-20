using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services;
using Models.Enums;
using Models.Requests;
using Models.Requests.Werewords;
using Models.Responses;
using Models.Responses.Werewords;
using Models.Responses.Werewords.Player;
using Models.Responses.Werewords.Round;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords
{
    public class RoundService
    {
        private readonly IRoundRepository _roundRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly PlayerStatusService _playerStatusService;
        private readonly WerewordsHubContext _werewordsHubContext;
        private readonly RequestLogger<RoundService> _logger;
        private static readonly Random s_Random = new Random();

        private const int DefaultRoundLength = 4;

        public RoundService(IRoundRepository roundRepository,
            IPlayerRepository playerRepository,
            ISessionRepository sessionRepository,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            PlayerStatusService playerStatusService,
            WerewordsHubContext werewordsHubContext,
            RequestLogger<RoundService> logger)
        {
            _roundRepository = roundRepository;
            _playerRepository = playerRepository;
            _sessionRepository = sessionRepository;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _playerStatusService = playerStatusService;
            _werewordsHubContext = werewordsHubContext;
            _logger = logger;
        }

        internal async Task CreateNewRoundAsync(Session session)
        {
            var players = await _playerRepository.FilterAsync(p => p.SessionId == session.Id);

            var mayor = players[s_Random.Next(players.Count)];
            var otherPlayers = players.Where(p => p.Id != mayor.Id);

            var round = new Round
            {
                Mayor = mayor,
                Session = session,
                RoundDurationMinutes = DefaultRoundLength,
                Status = RoundStatusEnum.NightSecretRole
            };

            await _roundRepository.InsertAsync(round);

            session.CurrentRound = round;

            await _sessionRepository.UpdateAsync(session);

            var secretRoles = PlayerCountConfiguration.GetShuffledSecretRolesList(players.Count);

            foreach (var player in players)
            {
                await _playerRoundInformationRepository.InsertAsync(new PlayerRoundInformation
                {
                    Player = player,
                    Round = round,
                    SecretRole = secretRoles.Pop(),
                    IsMayor = player.Id == mayor.Id
                });
            }

            await _werewordsHubContext.SendSecretRoleAsync(session.Id);
        }

        internal GenericResponse<WordChoiceResponse> GetWordChoiceAsync()
        {
            var words = StaticWordsList.GetWords(3, new[] { WordListEnum.JustOne, WordListEnum.Codenames, WordListEnum.CodenamesDuet });

            return GenericResponse<WordChoiceResponse>.Ok(new WordChoiceResponse { Words = words });
        }

        internal async Task<GenericResponseBase> PostWordAsync(WordChoiceRequest request)
        {
            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find session");
            }
            if (!session.CurrentRoundId.HasValue)
            {
                return GenericResponseBase.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.CurrentRoundId.Value);

            if (round == null)
            {
                return GenericResponseBase.Error("Unable to fetch current round");
            }
            if (!round.MayorId.HasValue)
            {
                return GenericResponseBase.Error("Round does not have a mayor!");
            }

            if (round.MayorId.Value != request.PlayerId)
            {
                return GenericResponseBase.Error("Player other than mayor gave a secret word");
            }

            round.SecretWord = request.Word;
            
            await _roundRepository.UpdateAsync(round);

            await _playerStatusService.UpdateAllPlayersForSessionAsync(session.Id, PlayerStatusEnum.NightSecretWord);

            await _werewordsHubContext.SendSecretWordAsync(session.Id);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponse<RoundResponse>> GetAsync(PlayerSessionRequest request)
        {
            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<RoundResponse>.Error("Unable to find session");
            }
            if (!session.CurrentRoundId.HasValue)
            {
                return GenericResponse<RoundResponse>.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.CurrentRoundId.Value);

            if (round == null)
            {
                return GenericResponse<RoundResponse>.Error("Unable to fetch current round");
            }
            if (!round.MayorId.HasValue)
            {
                return GenericResponse<RoundResponse>.Error("Round does not have a mayor!");
            }
            var players = await _playerRoundInformationRepository.GetForRoundAsync(round.Id);
            var currentPlayer = players.First(p => p.Id == request.PlayerId);                     

            return GenericResponse<RoundResponse>.Ok(new RoundResponse 
            {
                Id = round.Id,
                SecretWord = currentPlayer.SecretRole != SecretRolesEnum.Villager || currentPlayer.IsMayor ? round.SecretWord : null,
                Players = players.Where(p => !p.IsMayor).Select(p => new PlayerRoundInformationResponse 
                {
                    Id = p.Id,
                    Name = p.Player.Name,
                    Active = p.Player.Status == PlayerStatusEnum.DayActive,
                    Correct = p.Correct,
                    Crosses = p.Crosses,
                    Ticks = p.Ticks,
                    QuestionMarks = p.QuestionMarks,
                    SoClose = p.SoClose
                })
            });
        }
    }
}
