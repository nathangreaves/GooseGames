using Entities.Codenames;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services;
using Models.Enum;
using Models.Enums;
using Models.Requests.Codenames;
using Models.Responses;
using Models.Responses.Codenames;
using RepositoryInterface.Codenames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Codenames
{
    public class CodenamesService
    {
        private readonly ICodenamesRepository _codenamesRepository;
        private readonly CodenamesHubContext _codenamesHubContext;
        private readonly RequestLogger<CodenamesService> _logger;
        private static Random _random = new Random();

        public CodenamesService(ICodenamesRepository codenamesRepository, 
            CodenamesHubContext codenamesHubContext,
            RequestLogger<CodenamesService> logger)
        {
            _codenamesRepository = codenamesRepository;
            _codenamesHubContext = codenamesHubContext;
            _logger = logger;
        }

        public async Task<GenericResponse<Models.Responses.Codenames.Session>> GetAsync(SessionRequest request)
        {
            var session = await _codenamesRepository.GetSessionAndWordsAsync(request.GameIdentifier);
            if (session == null)
            {
                session = new Entities.Codenames.Session
                {
                    Password = request.GameIdentifier
                };

                await _codenamesRepository.InsertSessionAsync(session);

                await RefreshWords(session);
            }

            return GenericResponse<Models.Responses.Codenames.Session>.Ok(new Models.Responses.Codenames.Session 
            {
                SessionId = session.Id,
                Words = session.Words.Select(x => new Models.Responses.Codenames.CodenamesWord 
                {
                    Id = x.Id,
                    Revealed = x.Revealed,
                    Word = x.Word,
                    WordType = (WordType)(int)x.WordType
                }),
                BlueTurn = session.IsBlueTurn,
                GameOver = session.BlueVictory.HasValue,
                BlueVictory = session.BlueVictory.HasValue && session.BlueVictory.Value,
                BlueWordsRemaining = session.Words.Where(x => x.WordType == Entities.Codenames.WordTypeEnum.Blue && !x.Revealed).Count(),
                RedWordsRemaining = session.Words.Where(x => x.WordType == Entities.Codenames.WordTypeEnum.Red && !x.Revealed).Count()                
            });
        }

        public async Task<GenericResponseBase> RefreshWordsAsync(RefreshSessionRequest request)
        {
            var session = await _codenamesRepository.GetSessionAndWordsAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find Session to refresh");
            }

            await RefreshWords(session);

            await _codenamesHubContext.SendWordsRefreshedAsync(request.SessionId);

            return GenericResponseBase.Ok();
        }

        public async Task<GenericResponseBase> RevealWordAsync(RevealWordRequest request)
        {
            var session = await _codenamesRepository.GetSessionAndWordsAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find Session to refresh");
            }

            var word = await _codenamesRepository.GetWordAsync(request.WordId);
            if (word == null)
            {
                return GenericResponseBase.Error("Unable to find word");
            }

            if (word.SessionId != session.Id)
            {
                return GenericResponseBase.Error("Word does not match session");
            }

            if (word.SessionWordsId != session.SessionWordsId)
            {
                return GenericResponseBase.Error("Word does not match this current version of the session");
            }

            if (!word.Revealed)
            {
                var isBlueTurn = session.IsBlueTurn;
                word.Revealed = true;
                word.RevealedByBlue = isBlueTurn;

                switch(word.WordType)
                {
                    case WordTypeEnum.Neutral:
                        session.IsBlueTurn = !isBlueTurn;
                        break;
                    case WordTypeEnum.Red:
                        if (isBlueTurn)
                        {
                            session.IsBlueTurn = !isBlueTurn;
                        }
                        if (session.Words.Where(x => x.WordType == WordTypeEnum.Red && !x.Revealed).Count() == 0)
                        {
                            session.BlueVictory = false;                
                        }

                        break;
                    case WordTypeEnum.Blue:
                        if (!isBlueTurn)
                        {
                            session.IsBlueTurn = !isBlueTurn;
                        }
                        if (session.Words.Where(x => x.WordType == WordTypeEnum.Blue && !x.Revealed).Count() == 0)
                        {
                            session.BlueVictory = true;
                        }

                        break;
                    case WordTypeEnum.Bomb:

                        session.BlueVictory = !isBlueTurn;
                        break;
                }

                await _codenamesHubContext.SendRevealWordAsync(session.Id, word.Id);

                if (session.BlueVictory.HasValue)
                {
                    await _codenamesHubContext.SendIsBlueVictoryAsync(session.Id, session.BlueVictory.Value);
                }
                else if (session.IsBlueTurn != isBlueTurn)
                {
                    await _codenamesHubContext.SendIsBlueTurnAsync(session.Id, session.IsBlueTurn);
                }

                await _codenamesRepository.UpdateSessionAsync(session);
                await _codenamesRepository.UpdateWordAsync(word);
            }

            return GenericResponseBase.Ok();
        }

        public async Task<GenericResponseBase> PassTurnAsync(PassRequest request)
        {
            var session = await _codenamesRepository.GetSessionAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find Session to refresh");
            }

            session.IsBlueTurn = !session.IsBlueTurn;

            await _codenamesRepository.UpdateSessionAsync(session);

            await _codenamesHubContext.SendIsBlueTurnAsync(session.Id, session.IsBlueTurn);

            return GenericResponseBase.Ok();
        }

        private async Task RefreshWords(Entities.Codenames.Session session)
        {
            var words = StaticWordsList.GetWords(25, new[] { WordListEnum.Codenames, WordListEnum.CodenamesDuet, WordListEnum.CodenamesDeepUndercover });

            session.SessionWordsId = Guid.NewGuid();

            var wordList = words.Select(x => new Entities.Codenames.CodenamesWord 
            {
                Revealed = false,
                SessionId = session.Id,
                SessionWordsId = session.SessionWordsId,
                Word = x,
                WordType = Entities.Codenames.WordTypeEnum.Neutral
            }).ToList();
            session.IsBlueTurn = _random.Next(0, 2) == 0;
            session.BlueVictory = null;

            int blueWords = session.IsBlueTurn ? 9 : 8;
            int redWords = session.IsBlueTurn ? 8 : 9;

            AssignWordType(blueWords, Entities.Codenames.WordTypeEnum.Blue, wordList);
            AssignWordType(redWords, Entities.Codenames.WordTypeEnum.Red, wordList);
            AssignWordType(1, Entities.Codenames.WordTypeEnum.Bomb, wordList);

            session.Words = wordList;

            await _codenamesRepository.InsertWordsAsync(wordList);
            await _codenamesRepository.UpdateSessionAsync(session);
        }

        private static void AssignWordType(int numberOfWords, Entities.Codenames.WordTypeEnum wordType, List<Entities.Codenames.CodenamesWord> words)
        {
            int count = numberOfWords;

            while (count > 0)
            {
                var word = words[_random.Next(0, 25)];
                if (word.WordType == Entities.Codenames.WordTypeEnum.Neutral)
                {
                    word.WordType = wordType;
                    count = count -= 1;
                }
            }
        }
    }
}
