using Entities.LetterJam;
using GooseGames.Hubs;
using GooseGames.Logging;
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
    public class FinalWordService
    {
        private readonly IFinalWordLetterRepository _finalWordLetterRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly MyJamService _myJamService;
        private readonly LetterJamHubContext _letterJamHubContext;
        private readonly RequestLogger<FinalWordService> _logger;

        public FinalWordService(IFinalWordLetterRepository finalWordLetterRepository,
            ILetterCardRepository letterCardRepository,
            MyJamService myJamService,
            LetterJamHubContext letterJamHubContext,
            RequestLogger<FinalWordService> logger)
        {
            _finalWordLetterRepository = finalWordLetterRepository;
            _letterCardRepository = letterCardRepository;
            _myJamService = myJamService;
            _letterJamHubContext = letterJamHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponseBase> PostAsync(FinalWordRequest request)
        {
            var response = await _myJamService.PostLetterGuessesAsync(new MyJamLetterGuessesRequest
            {
                GameId = request.GameId,
                LetterGuesses = request.LetterGuesses,
                MoveOnToNextLetter = false,
                PlayerId = request.PlayerId,
                SessionId = request.SessionId
            });
            if (!response.Success)
            {
                return response;
            }

            await RemoveFinalWordLettersAsync(request);
            bool claimWildcard = false;
            List<Guid> bonusCardsClaimed = new List<Guid>();

            if (request.FinalWordLetters.Any(l => l.IsWildCard))
            {
                if (await _finalWordLetterRepository.CountAsync(c => c.GameId == request.GameId && c.Wildcard) > 0)
                {
                    //Wildard already in use
                    return GenericResponseBase.Error("9566FE53-76E7-4F81-A295-9052D7C03CA8");
                }
                claimWildcard = true;
            }
            var actualLetterIds = request.FinalWordLetters.Where(x => x.LetterId.HasValue).Select(x => x.LetterId.Value).ToList();
            var actualLetters = await _letterCardRepository.FilterAsync(lC => actualLetterIds.Contains(lC.Id));
            foreach (var actualLetter in actualLetters)
            {
                if (actualLetter.BonusLetter && await _finalWordLetterRepository.CountAsync(c => c.GameId == request.GameId && c.CardId == actualLetter.Id) > 0)
                {
                    //Bonus letter already in use
                    return GenericResponseBase.Error("C0FBCE81-207F-4DE6-A953-1D9F66AA9279");
                }
                bonusCardsClaimed.Add(actualLetter.Id);
            }
            var actualLettersDictionary = actualLetters.ToDictionary(aL => aL.Id, aL => aL);

            var index = 0;
            var finalWordLetters = request.FinalWordLetters.Select(cL =>
            {
                var letter = !cL.IsWildCard ? actualLettersDictionary[cL.LetterId.Value] : null;
                return new FinalWordLetter
                {
                    Id = Guid.NewGuid(),
                    BonusLetter = letter?.BonusLetter ?? false,
                    CardId = letter?.Id,
                    GameId = request.GameId,
                    Letter = letter?.Letter,
                    PlayerLetterGuess = letter?.PlayerLetterGuess,
                    LetterIndex = index++,
                    PlayerId = request.PlayerId,
                    Wildcard = cL.IsWildCard                    
                };
            });

            await _finalWordLetterRepository.InsertRangeAsync(finalWordLetters);

            if (claimWildcard)
            {
                await _letterJamHubContext.SendFinalWordLetterClaimedAsync(request.SessionId, null, true);
            }
            foreach (var bonusLetterClaim in bonusCardsClaimed)
            {
                await _letterJamHubContext.SendFinalWordLetterClaimedAsync(request.SessionId, bonusLetterClaim, false);
            }

            return GenericResponseBase.Ok();
        }

        private async Task RemoveFinalWordLettersAsync(FinalWordRequest request)
        {
            var previousFinalWordLetters = await _finalWordLetterRepository.FilterAsync(l => l.PlayerId == request.PlayerId);
            foreach (var letter in previousFinalWordLetters)
            {
                if (letter.Wildcard)
                {
                    await _letterJamHubContext.SendFinalWordLetterReturnedAsync(request.SessionId, null, true);
                }
                if (letter.BonusLetter)
                {
                    await _letterJamHubContext.SendFinalWordLetterReturnedAsync(request.SessionId, letter.CardId, false);
                }

                await _finalWordLetterRepository.DeleteAsync(letter);
            }
        }

        internal async Task<GenericResponse<IEnumerable<FinalWordPublicCardResponse>>> GetFinalWordBonusCardsAsync(PlayerSessionGameRequest request)
        {
            var preSelected = await _finalWordLetterRepository.FilterAsync(fW => fW.GameId == request.GameId && (fW.BonusLetter || fW.Wildcard));
            var wildcardUsed = preSelected.Any(c => c.Wildcard);
            var preselectedIds = preSelected.Where(x => x.CardId.HasValue).Select(x => x.CardId.Value);

            var bonusLetters = await _letterCardRepository.FilterAsync(lC => lC.GameId == request.GameId && lC.BonusLetter && lC.PlayerId == null && !lC.Discarded);

            List<FinalWordPublicCardResponse> response = new List<FinalWordPublicCardResponse>();
            if (!wildcardUsed)
            {
                response.Add(new FinalWordPublicCardResponse
                {
                    CardId = null,
                    IsWildCard = true,
                    Letter = null
                });
            }
            foreach (var bonusLetterCard in bonusLetters.Where(bL => !preselectedIds.Contains(bL.Id)))
            {
                response.Add(new FinalWordPublicCardResponse
                {
                    CardId = bonusLetterCard.Id,
                    IsWildCard = false,
                    Letter = bonusLetterCard.Letter
                });
            }
            return GenericResponse<IEnumerable<FinalWordPublicCardResponse>>.Ok(response);
        }
    }
}
