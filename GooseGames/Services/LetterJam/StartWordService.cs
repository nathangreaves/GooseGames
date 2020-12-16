using Entities.LetterJam;
using Entities.LetterJam.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Enums;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using Models.Responses.LetterJam;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class StartWordService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly PlayerService _playerService;
        private readonly PlayerStatusService _playerStatusService;
        private readonly GameService _gameService;
        private readonly LetterJamHubContext _letterJamHubContext;
        private readonly RequestLogger<StartWordService> _logger;

        private static readonly Random s_Random = new Random();

        public StartWordService(
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            ILetterCardRepository letterCardRepository,            
            PlayerService playerService,
            PlayerStatusService playerStatusService,
            GameService gameService,
            LetterJamHubContext letterJamHubContext,
            RequestLogger<StartWordService> logger
            )
        {
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _letterCardRepository = letterCardRepository;
            _playerService = playerService;
            _playerStatusService = playerStatusService;
            _gameService = gameService;
            _letterJamHubContext = letterJamHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponse<StartWordConfigurationResponse>> GetStartWordConfigurationAsync(PlayerSessionGameRequest request)
        {
            var game = await _gameRepository.GetAsync(request.GameId);
            if (game == null)
            {
                return GenericResponse<StartWordConfigurationResponse>.Error("Unable to find game");
            }

            var players = await _playerService.GetForSessionAsync(game.SessionId);
            if (!players.Any(p => p.Id == request.PlayerId))
            {
                return GenericResponse<StartWordConfigurationResponse>.Error("Unable to find player");
            }
            var currentPlayer = players.First(p => p.Id == request.PlayerId);
            var nextPlayer = players.Where(p => p.PlayerNumber > currentPlayer.PlayerNumber).OrderBy(p => p.PlayerNumber).FirstOrDefault();
            if (nextPlayer == null)
            {
                nextPlayer = players.OrderBy(p => p.PlayerNumber).First();
            }

            var playerState = await _playerStateRepository.SingleOrDefaultAsync(pS => pS.PlayerId == nextPlayer.Id);
            if (playerState == null)
            {
                return GenericResponse<StartWordConfigurationResponse>.Error("Unable to find next player in game");
            }

            return GenericResponse<StartWordConfigurationResponse>.Ok(new StartWordConfigurationResponse
            {
                ForPlayerId = nextPlayer.Id,
                NumberOfLetters = playerState.OriginalWordLength
            });
        }

        internal GenericResponse<RandomWordResponse> GetRandomWord(RandomWordRequest request)
        {
            return GenericResponse<RandomWordResponse>.Ok(new RandomWordResponse
            {
                RandomWord = StaticWordsList.GetWords(1, new[] { WordListEnum.JustOne, WordListEnum.Codenames, WordListEnum.CodenamesDuet }, request.NumberOfLetters).First()
            });
        }


        internal async Task<GenericResponseBase> PostStartWordAsync(StartWordRequest request)
        {
            if (await ReserveLettersAsync(request.GameId, request.ForPlayerId, request.StartWord))
            {
                await _playerStatusService.UpdatePlayerToStatusAsync(request, PlayerStatus.WaitingForFirstRound);

                //TODO: this message is now redundant as the above sends the message 'playerStatus'
                await _letterJamHubContext.SendPlayerHasChosenStartingWordAsync(request.SessionId, request.PlayerId);

                if (await _playerStatusService.AllPlayersMatchStatusAsync(request.GameId, PlayerStatus.WaitingForFirstRound))
                {

                    var players = await _playerStateRepository.FilterAsync(p => p.GameId == request.GameId);
                    var playerIds = players.Select(p => p.PlayerId).ToList();
                    var letters = await _letterCardRepository.FilterAsync(p => p.GameId == request.GameId && p.PlayerId.HasValue && playerIds.Contains(p.PlayerId.Value));
                    var groupedLetters = letters.GroupBy(l => l.PlayerId).ToDictionary(d => d.Key, d => d);

                    foreach (var player in players)
                    {
                        if (!groupedLetters.ContainsKey(player.PlayerId) || groupedLetters[player.PlayerId].Count() != player.OriginalWordLength)
                        {
                            await _letterCardRepository.UnreserveAllCardsForGameAsync(request.GameId);
                            await _playerStatusService.UpdateAllPlayersForGameAsync(request.GameId, PlayerStatus.ConstructingWord);
                            await _letterJamHubContext.SendBeginSessionAsync(request.SessionId, request.GameId);

                            return GenericResponseBase.Error("Not all players had correct word length");
                        }
                    }

                    await _gameService.FinishGameSetupAsync(request.SessionId, request.GameId);
                }

                return GenericResponseBase.Ok();
            }

            //Unable to reserve requested letters
            return GenericResponseBase.Error("182ef180-38a9-470f-a217-17566a99ab57");
        }

        internal async Task<GenericResponse<IEnumerable<Models.Responses.LetterJam.PlayerActionResponse>>> GetPlayerActionsAsync(PlayerSessionGameRequest request)
        {
            return await _playerStatusService.GetPlayerActionsAsync(request, PlayerStatus.WaitingForFirstRound);
        }
        
        
        private async Task<bool> ReserveLettersAsync(Guid gameId, Guid reserveForPlayerId, string word)
        {
            var listOfChars = word.ToList();
            var matchingLetters = await _letterCardRepository.FilterAsync(lC => lC.GameId == gameId && listOfChars.Contains(lC.Letter) && lC.PlayerId == null);

            var reservedLetters = new List<LetterCard>();

            var list = new List<int>();
            for (int i = 0; i < word.Length; i++)
            {
                list.Add(i);
            }
            var order = new Stack<int>(list.OrderBy(l => s_Random.Next(0, int.MaxValue)));

            foreach (char c in word)
            {
                var letter = matchingLetters.FirstOrDefault(l => l.Letter == c);
                if (letter == null)
                {
                    return false;
                }
                matchingLetters.Remove(letter);
                reservedLetters.Add(letter);

                letter.PlayerId = reserveForPlayerId;
                letter.LetterIndex = order.Pop();
            }

            await _letterCardRepository.UpdateRangeAsync(reservedLetters);

            var firstLetter = reservedLetters.First(l => l.LetterIndex == 0);
            var playerState = await _playerStateRepository.SingleOrDefaultAsync(pS => pS.PlayerId == reserveForPlayerId);
            playerState.CurrentLetterId = firstLetter.Id;
            playerState.CurrentLetterIndex = firstLetter.LetterIndex;
            await _playerStateRepository.UpdateAsync(playerState);

            return true;
        }
    }
}
