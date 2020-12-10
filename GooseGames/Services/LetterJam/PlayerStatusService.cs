using Entities.LetterJam;
using Entities.LetterJam.Enums;
using GooseGames.Hubs;
using GooseGames.Services.Global;
using Microsoft.EntityFrameworkCore.Storage;
using Models.HubMessages.LetterJam;
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
        private readonly IRoundRepository _roundRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IClueLetterRepository _clueLetterRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly INonPlayerCharacterRepository _nonPlayerCharacterRepository;
        private readonly LetterJamHubContext _letterJamHubContext;

        public PlayerStatusService(
            Global.SessionService sessionService,
            Global.GlobalPlayerStatusService playerStatusService,       
            IRoundRepository roundRepository,
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            IClueLetterRepository clueLetterRepository,
            ILetterCardRepository letterCardRepository,
            INonPlayerCharacterRepository nonPlayerCharacterRepository,
            LetterJamHubContext letterJamHubContext)
        {
            _sessionService = sessionService;
            _playerStatusService = playerStatusService;
            _roundRepository = roundRepository;
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _clueLetterRepository = clueLetterRepository;
            _letterCardRepository = letterCardRepository;
            _nonPlayerCharacterRepository = nonPlayerCharacterRepository;
            _letterJamHubContext = letterJamHubContext;
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

        internal async Task<GenericResponseBase> SetUndoWaitingForNextRoundAsync(PlayerSessionGameRequest request)
        {
            await UpdatePlayerToStatusAsync(request.PlayerId, request.GameId, PlayerStatus.ReceivedClue);
            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> SetWaitingForNextRoundAsync(PlayerSessionGameRequest request)
        {
            await UpdatePlayerToStatusAsync(request.PlayerId, request.GameId, PlayerStatus.ReadyForNextRound);

            if (await AllPlayersMatchStatusAsync(request.GameId, PlayerStatus.ReadyForNextRound))
            {
                var game = await _gameRepository.GetAsync(request.GameId);

                var currentRound = await _roundRepository.GetAsync(game.CurrentRoundId.Value);

                var currentRoundId = game.CurrentRoundId.Value;
                var currentRoundClueId = currentRound.ClueId.Value;
                
                await UpdateNpcCardsForClueAsync(request, currentRoundClueId, game);                

                var players = await _playerStateRepository.GetPlayerStatesAndCardsForGame(request.GameId);

                if (players.All(p => p.CurrentLetterId == null || p.CurrentLetter.BonusLetter))
                {
                    //TODO: Trigger game end
                }
                else
                {
                    //TODO: Give players without a letter a new bonus letter.
                    await AssignNewBonusLettersAsync(request, players);


                    var newRound = new Round
                    {
                        Id = Guid.NewGuid(),
                        GameId = game.Id,
                        RoundNumber = currentRound.RoundNumber + 1,
                        RoundStatus = RoundStatus.ProposingClues
                    };
                    await _roundRepository.InsertAsync(newRound);

                    game.CurrentRoundId = newRound.Id;
                    await _gameRepository.UpdateAsync(game);

                    await UpdateAllPlayersForGameAsync(game.Id, PlayerStatus.ProposingClues);

                    await _letterJamHubContext.SendBeginNewRoundAsync(request.SessionId, newRound.Id);
                }

            }
            return GenericResponseBase.Ok();
        }

        private async Task AssignNewBonusLettersAsync(PlayerSessionGameRequest request, IList<PlayerState> players)
        {
            var cards = new List<LetterCard>();
            foreach (var player in players.Where(p => p.CurrentLetterId == null))
            {
                var card = await _letterCardRepository.GetNextUndiscardedCardAsync(request.GameId);
                card.PlayerId = player.PlayerId;
                card.BonusLetter = true;
                await _letterCardRepository.UpdateAsync(card);

                player.CurrentLetter = card;
                player.CurrentLetterId = card.Id;
                player.CurrentLetterIndex = null;

                await _playerStateRepository.UpdateAsync(player);
                await _letterJamHubContext.SendNewBonusCardAsync(request.SessionId, new LetterCardResponse { 
                    BonusLetter = true,
                    CardId = card.Id,
                    Letter = card.Letter, 
                    PlayerId = player.PlayerId
                });
            }
        }

        private async Task UpdateNpcCardsForClueAsync(PlayerSessionGameRequest request, Guid currentRoundClueId, Game game)
        {
            var currentClueNpcLetters = await _clueLetterRepository.GetNonPlayerCharacterLettersUsedForClueAsync(currentRoundClueId);
            var npcLetterCards = currentClueNpcLetters.Select(s => s.LetterCard).Distinct();
            var npcIds = currentClueNpcLetters.Select(s => s.NonPlayerCharacterId.Value).Distinct();

            foreach (var npcCard in npcLetterCards)
            {
                npcCard.NonPlayerCharacterId = null;
                npcCard.Discarded = true;
            }
            await _letterCardRepository.UpdateRangeAsync(npcLetterCards);

            var unlockTokensFromNonPlayerCharacterIds = new List<Guid>();

            var npcs = new List<NonPlayerCharacter>();
            foreach (var npcId in npcIds)
            {
                var npc = await _nonPlayerCharacterRepository.GetAsync(npcId);
                LetterCard nextNpcCard;
                if (!npc.ClueReleased)
                {
                    nextNpcCard = await _letterCardRepository.GetNextNpcCardAsync(npcId, npc.CurrentLetterId.Value);

                    npc.CurrentLetterId = nextNpcCard.Id;
                    npc.NumberOfLettersRemaining -= 1;
                    if (npc.NumberOfLettersRemaining == 0)
                    {
                        npc.ClueReleased = true;

                        game.LockedCluesRemaining -= 1;
                        game.GreenCluesRemaining += 1;

                        unlockTokensFromNonPlayerCharacterIds.Add(npc.Id);
                    }
                }
                else
                {
                    nextNpcCard = await _letterCardRepository.GetNextUndiscardedCardAsync(request.GameId);
                    nextNpcCard.NonPlayerCharacterId = npc.Id;

                    await _letterCardRepository.UpdateAsync(nextNpcCard);

                    npc.CurrentLetterId = nextNpcCard.Id;
                }

                await _letterJamHubContext.SendNewNpcCardAsync(request.SessionId, new LetterCardResponse
                {
                    BonusLetter = true,
                    CardId = nextNpcCard.Id,
                    Letter = nextNpcCard.Letter,
                    NonPlayerCharacterId = npc.Id
                });

                npcs.Add(npc);
            }
            await _nonPlayerCharacterRepository.UpdateRangeAsync(npcs);
            if (unlockTokensFromNonPlayerCharacterIds.Any())
            {
                var tokenUpdate = new TokenUpdate
                {
                    UnlockTokensFromNonPlayerCharacterIds = unlockTokensFromNonPlayerCharacterIds
                };
                await _letterJamHubContext.SendTokenUpdate(request.SessionId, tokenUpdate); 
            }
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
