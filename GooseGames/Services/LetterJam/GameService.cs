using Entities.LetterJam;
using Entities.LetterJam.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Responses;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;
        private IRoundRepository _roundRepository;
        private readonly INonPlayerCharacterRepository _nonPlayerCharacterRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly PlayerService _playerService;
        private readonly PlayerStatusService _playerStatusService;
        private readonly LetterJamHubContext _letterJamHubContext;
        private readonly RequestLogger<GameService> _logger;

        public GameService(
            IGameRepository gameRepository,
            IRoundRepository roundRepository,
            INonPlayerCharacterRepository nonPlayerCharacterRepository,
            ILetterCardRepository letterCardRepository,
            PlayerService playerService,
            PlayerStatusService playerStatusService,
            LetterJamHubContext letterJamHubContext,
            RequestLogger<GameService> logger
            )
        {
            _gameRepository = gameRepository;
            _roundRepository = roundRepository;
            _nonPlayerCharacterRepository = nonPlayerCharacterRepository;
            _letterCardRepository = letterCardRepository;
            _playerService = playerService;
            _playerStatusService = playerStatusService;
            _letterJamHubContext = letterJamHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponseBase> FinishGameSetupAsync(Guid sessionId, Guid gameId)
        {
            var game = await _gameRepository.GetAsync(gameId);
            if (game == null)
            {
                return GenericResponseBase.Error("Unable to find game");
            }

            var players = await _playerService.GetForSessionAsync(sessionId);
            var reservedEmojis = players.Select(p => p.Emoji).ToList();

            var round = new Round
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                RoundNumber = 1,
                RoundStatus = Entities.LetterJam.Enums.RoundStatus.ProposingClues
            };
            await _roundRepository.InsertAsync(round);

            game.CurrentRoundId = round.Id;
            game.GameStatus = Entities.LetterJam.Enums.GameStatus.ProposingClues;

            await _gameRepository.UpdateAsync(game);

            var numberOfPlayers = players.Count;

            var gameConfiguration = GameConfigurationService.GetForPlayerCount(numberOfPlayers);

            if (gameConfiguration.NonPlayerCharacters.Count > 0)
            {
                List<NonPlayerCharacter> nPCs = new List<NonPlayerCharacter>();

                foreach (var npc in gameConfiguration.NonPlayerCharacters)
                {
                    string emoji = null;
                    while (emoji == null)
                    {
                        var randomEmoji = _playerService.GetRandomEmoji();
                        if (!reservedEmojis.Contains(randomEmoji))
                        {
                            emoji = randomEmoji;
                        }
                    }
                    reservedEmojis.Add(emoji);

                    var npcEntity = new NonPlayerCharacter
                    {
                        Id = Guid.NewGuid(),
                        GameId = gameId,
                        Emoji = emoji,
                        Name = "NPC",
                        NumberOfLettersRemaining = npc.NumberOfLetters,
                        PlayerNumber = await _playerService.GetNextPlayerNumberForSessionAsync(sessionId)
                    };
                    await _nonPlayerCharacterRepository.InsertAsync(npcEntity);

                    await _letterCardRepository.ReserveLettersForNonPlayerCharacterAsync(npcEntity);
                }
            }

            await _playerStatusService.UpdateAllPlayersForGameAsync(gameId, PlayerStatus.ProposingClues);
            await _letterJamHubContext.SendBeginNewRoundAsync(sessionId, round.Id);

            return GenericResponseBase.Ok();
        }
    }
}
