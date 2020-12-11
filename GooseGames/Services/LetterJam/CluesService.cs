using Entities.LetterJam;
using Entities.LetterJam.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.HubMessages.LetterJam;
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
    public class CluesService
    {
        private readonly GameService _gameService;
        private readonly IClueRepository _clueRepository;
        private readonly IClueLetterRepository _clueLetterRepository;
        private readonly IClueVoteRepository _clueVoteRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly PlayerStatusService _playerStatusService;
        private readonly LetterJamHubContext _letterJamHubContext;
        private readonly RequestLogger<CluesService> _logger;

        public CluesService(
            GameService gameService,
            IClueRepository clueRepository,
            IClueLetterRepository clueLetterRepository,
            IClueVoteRepository clueVoteRepository,
            ILetterCardRepository letterCardRepository,
            IGameRepository gameRepository,
            IRoundRepository roundRepository,
            IPlayerStateRepository playerStateRepository,
            PlayerStatusService playerStatusService,
            LetterJamHubContext letterJamHubContext,
            RequestLogger<CluesService> logger)
        {
            _gameService = gameService;
            _clueRepository = clueRepository;
            _clueLetterRepository = clueLetterRepository;
            _clueVoteRepository = clueVoteRepository;
            _letterCardRepository = letterCardRepository;
            _gameRepository = gameRepository;
            _roundRepository = roundRepository;
            _playerStateRepository = playerStateRepository;
            _playerStatusService = playerStatusService;
            _letterJamHubContext = letterJamHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponse<ProposedCluesResponse>> GetProposedCluesAsync(RoundRequest request)
        {
            var nullableRoundId = request.RoundId ?? await _gameService.GetRoundIdAsync(request);
            if (!nullableRoundId.HasValue)
            {
                return GenericResponse<ProposedCluesResponse>.Error("Unable to get roundId");
            }
            var roundId = nullableRoundId.Value;

            var roundStatus = await _roundRepository.GetPropertyAsync(roundId, r => r.RoundStatus);
            if (roundStatus == RoundStatus.ReceivedClue)
            {
                return GenericResponse<ProposedCluesResponse>.Ok(new ProposedCluesResponse
                {
                    Clues = new List<ProposedClueResponse>(),
                    RoundStatus = RoundStatusEnum.ReceivedClue
                });
            }

            var clues = await _clueRepository.FilterAsync(c => c.RoundId == roundId);
            var clueVotes = await _clueVoteRepository.FilterAsync(cV => cV.RoundId == roundId);

            var clueVotesGrouped = clueVotes.GroupBy(c => c.ClueId).ToDictionary(c => c.Key, c => c);

            var numberOfPlayers = await _gameRepository.GetPropertyAsync(request.GameId, g => g.NumberOfPlayers);

            return GenericResponse<ProposedCluesResponse>.Ok(new ProposedCluesResponse 
            { 
                RoundStatus = RoundStatusEnum.ProposingClues,
                Clues = clues.Select(clue =>
                {
                    var votes = (clueVotesGrouped.ContainsKey(clue.Id) ? clueVotesGrouped[clue.Id] : (IEnumerable<ClueVote>)new List<ClueVote>())
                    .Select(clueVote => {
                        return new ClueVoteResponse
                        {
                            Id = clueVote.Id,
                            ClueId = clueVote.ClueId,
                            PlayerId = clueVote.PlayerId
                        };
                    }).ToList();

                    return new ProposedClueResponse
                    {
                        PlayerId = clue.ClueGiverPlayerId,
                        Id = clue.Id,
                        NumberOfLetters = clue.NumberOfLetters,
                        NumberOfPlayerLetters = clue.NumberOfPlayerLetters,
                        NumberOfNonPlayerLetters = clue.NumberOfNonPlayerLetters,
                        NumberOfBonusLetters = clue.NumberOfBonusLetters,
                        WildcardUsed = clue.WildcardUsed,
                        Votes = votes,
                        VoteSuccess = votes.Count == numberOfPlayers
                    };
                })
            });
        }


        internal async Task<GenericResponseBase> SubmitClueAsync(SubmitClueRequest request)
        {
            var roundId = request.RoundId;
            if (roundId == null)
            {
                var game = await _gameRepository.GetAsync(request.GameId);
                roundId = game.CurrentRoundId;
            }
            if (roundId == null)
            {
                return GenericResponseBase.Error("Unable to find current round id");
            }

            Round round = await _roundRepository.GetAsync(roundId.Value);
            if (round == null)
            {
                return GenericResponseBase.Error("Unable to find round");
            }

            var requestedClueLetters = request.ClueLetters;
            var clue = new Clue
            {
                Id = Guid.NewGuid(),
                ClueGiverPlayerId = request.PlayerId,
                RoundId = round.Id,
                RoundNumber = round.RoundNumber,
                WildcardUsed = requestedClueLetters.Any(c => c.IsWildCard)
            };
            var clueLetters = new List<ClueLetter>();

            var actualLetterIds = request.ClueLetters.Where(x => x.LetterId.HasValue).Select(x => x.LetterId).ToList();

            var letters = await _letterCardRepository.FilterAsync(c => actualLetterIds.Contains(c.Id));

            var letterIndex = 0;

            HashSet<Guid> playerIds = new HashSet<Guid>();
            HashSet<Guid> nonPlayerIds = new HashSet<Guid>();
            HashSet<Guid> bonusLetterIds = new HashSet<Guid>();

            foreach (var clueLetter in requestedClueLetters)
            {
                LetterCard letter = null;
                if (!clueLetter.IsWildCard)                
                {
                    letter = letters.FirstOrDefault(l => l.Id == clueLetter.LetterId);
                    if (letter == null)
                    {
                        return GenericResponseBase.Error("Unable to find letter");
                    }
                }

                ClueLetter newClueLetter = new ClueLetter
                {
                    Id = Guid.NewGuid(),
                    Clue = clue,
                    Letter = letter?.Letter,
                    PlayerId = letter?.PlayerId,
                    NonPlayerCharacterId = letter?.NonPlayerCharacterId,
                    LetterCardId = letter?.Id,
                    BonusLetter = letter?.BonusLetter ?? false,
                    LetterIndex = letterIndex,
                    IsWildCard = clueLetter.IsWildCard
                };
                clueLetters.Add(newClueLetter);
                
                if (newClueLetter.PlayerId.HasValue)
                {
                    playerIds.Add(newClueLetter.PlayerId.Value);
                }
                else if (newClueLetter.NonPlayerCharacterId.HasValue)
                {
                    nonPlayerIds.Add(newClueLetter.NonPlayerCharacterId.Value);
                }
                else if (newClueLetter.BonusLetter)
                {
                    bonusLetterIds.Add(newClueLetter.LetterCardId.Value);
                }
                letterIndex++;
            }

            clue.NumberOfPlayerLetters = playerIds.Count;
            clue.NumberOfNonPlayerLetters = nonPlayerIds.Count;
            clue.NumberOfBonusLetters = bonusLetterIds.Count;
            clue.NumberOfLetters = requestedClueLetters.Count();

            await _clueRepository.InsertAsync(clue);
            await _clueLetterRepository.InsertRangeAsync(clueLetters);

            await _letterJamHubContext.SendNewClueAsync(request.SessionId, new ProposedClueResponse
            {
                Id = clue.Id,
                PlayerId = clue.ClueGiverPlayerId,
                NumberOfLetters = clue.NumberOfLetters,
                NumberOfPlayerLetters = clue.NumberOfPlayerLetters,
                NumberOfNonPlayerLetters = clue.NumberOfNonPlayerLetters,
                NumberOfBonusLetters = clue.NumberOfNonPlayerLetters,
                WildcardUsed = clue.WildcardUsed
            });

            return GenericResponseBase.Ok();
        }
        internal async Task<GenericResponseBase> DeleteClueAsync(ClueRequest request)
        {
            if (!request.ClueId.HasValue)
            {
                return GenericResponseBase.Error("Clue id not provided");
            }

            var clue = await _clueRepository.GetAsync(request.ClueId.Value);
            if (clue.ClueGiverPlayerId != request.PlayerId)
            {
                return GenericResponseBase.Error("Unable to remove clue of another player");
            }
            if (request.RoundId.HasValue && request.RoundId != clue.RoundId)
            {
                return GenericResponseBase.Error("Incorrect round id");
            }
            await _letterJamHubContext.SendClueRemovedAsync(request.SessionId, request.ClueId.Value);

            var clueLetters = await _clueLetterRepository.FilterAsync(cL => cL.ClueId == clue.Id);
            foreach (var clueLetter in clueLetters)
            {
                await _clueLetterRepository.DeleteAsync(clueLetter);
            }

            var clueVotes = await _clueVoteRepository.FilterAsync(cV => cV.ClueId == clue.Id);
            foreach (var clueVote in clueVotes)
            {
                await _clueVoteRepository.DeleteAsync(clueVote);
            }

            await _clueRepository.DeleteAsync(clue);

            return GenericResponseBase.Ok();
        }


        internal async Task<GenericResponse<IEnumerable<ClueLetterResponse>>> GetLettersForClueAsync(ClueRequest request)
        {
            if (!request.ClueId.HasValue)
            {
                return GenericResponse<IEnumerable<ClueLetterResponse>>.Error("Clue id not provided");
            }
            var cards = await _clueLetterRepository.FilterAsync(l => l.ClueId == request.ClueId.Value);
            var orderedCards = cards.OrderBy(c => c.LetterIndex);
            
            return GenericResponse<IEnumerable<ClueLetterResponse>>.Ok(orderedCards.Select(c =>
            {
                return new ClueLetterResponse
                {
                    CardId = c.LetterCardId,
                    BonusLetter = c.BonusLetter,
                    Letter = c.PlayerId != request.PlayerId ? c.Letter : null,
                    IsWildCard = c.IsWildCard,
                    PlayerId = c.PlayerId,
                    NonPlayerCharacterId = c.NonPlayerCharacterId                    
                };
            }));
        }

        internal async Task<GenericResponseBase> GiveClueAsync(ClueRequest request)
        {
            var roundId = request.RoundId;
            if (!roundId.HasValue)
            {
                roundId = await _gameRepository.GetPropertyAsync(request.GameId, g => g.CurrentRoundId);
            }
            if (!roundId.HasValue)
            {
                return GenericResponseBase.Error("Unable to find round");
            }
            if (request.ClueId == null)
            {
                return GenericResponseBase.Error("Clue id not provided");
            }

            var game = await _gameRepository.GetAsync(request.GameId);

            var clue = await _clueRepository.GetAsync(request.ClueId.Value);

            var playerState = await _playerStateRepository.SingleOrDefaultAsync(s => s.PlayerId == clue.ClueGiverPlayerId);

            var tokenUpdate = new TokenUpdate();

            var gameConfig = GameConfigurationService.GetForPlayerCount(game.NumberOfPlayers);
            if (playerState.NumberOfCluesGiven < gameConfig.NumberOfRedCluesPerPlayer)
            {
                game.RedCluesRemaining -= 1;
                tokenUpdate.AddRedTokenToPlayerId = clue.ClueGiverPlayerId;

                if (game.RedCluesRemaining == 0)
                {
                    int numberOfGreenTokensToUnlock = gameConfig.NumberOfLockedGreenClues - gameConfig.NonPlayerCharacters.Count;
                    game.GreenCluesRemaining += numberOfGreenTokensToUnlock;
                    game.LockedCluesRemaining -= numberOfGreenTokensToUnlock;

                    tokenUpdate.UnlockTokensFromSupply = numberOfGreenTokensToUnlock;
                }
            }
            else
            {
                game.GreenCluesRemaining -= 1;
                tokenUpdate.AddGreenTokenToPlayerId = clue.ClueGiverPlayerId;
            }
            await _gameRepository.UpdateAsync(game);

            playerState.NumberOfCluesGiven += 1;

            await _letterJamHubContext.SendTokenUpdate(request.SessionId, tokenUpdate);
            await _playerStateRepository.UpdateAsync(playerState);

            var round = await _roundRepository.GetAsync(roundId.Value);
            round.ClueGiverPlayerId = clue.ClueGiverPlayerId;
            round.ClueId = clue.Id;
            round.RoundStatus = RoundStatus.ReceivedClue;
            await _roundRepository.UpdateAsync(round);

            await _playerStatusService.UpdateAllPlayersForGameAsync(request.GameId, PlayerStatus.ReceivedClue);

            var letters = await _clueLetterRepository.GetForCluesAsync(new[] { clue.Id });

            await _letterJamHubContext.GiveClueAsync(request.SessionId, new MyJamRound { 
                ClueGiverPlayerId = clue.ClueGiverPlayerId,
                ClueId = clue.Id,
                RequestingPlayerReceivedClue = null,
                Letters = letters[clue.Id].OrderBy(l => l.LetterIndex).Select(s => {
                    return new ClueLetterResponse
                    {
                        CardId = s.LetterCardId,
                        Letter = s.Letter,
                        PlayerId = s.PlayerId,
                        NonPlayerCharacterId = s.NonPlayerCharacterId,
                        BonusLetter = s.BonusLetter,
                        IsWildCard = s.IsWildCard
                    };
                })
            });

            return GenericResponseBase.Ok();
        }
    }
}
