using GooseGames.Logging;
using Models.Requests;
using Models.Responses;
using Models.Responses.Fuji.Hands;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Fuji
{
    public class HandService
    {
        private readonly IHandCardRepository _handCardRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly RequestLogger<HandService> _logger;

        public HandService(IHandCardRepository handCardRepository,
            IPlayerRepository playerRepository,
            RequestLogger<HandService> logger)
        {
            _handCardRepository = handCardRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerHand>> GetPlayerHandAsync(PlayerSessionRequest request)
        {
            var playedCardId = await _playerRepository.GetPropertyAsync(request.PlayerId, p => p.PlayedCardId);

            var cards = await _handCardRepository.FilterAsync(c => c.SessionId == request.SessionId && c.PlayerId == request.PlayerId && (playedCardId == null || playedCardId != c.Id));

            return GenericResponse<PlayerHand>.Ok(new PlayerHand 
            {
                Cards = cards.OrderBy(c => c.FaceValue).Select(c => new Models.Responses.Fuji.Cards.Card 
                { 
                    Id = c.Id,
                    FaceValue = c.FaceValue
                })
            });
        }
    }
}
