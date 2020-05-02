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
        private readonly RequestLogger<HandService> _logger;

        public HandService(IHandCardRepository handCardRepository,
            RequestLogger<HandService> logger)
        {
            _handCardRepository = handCardRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerHand>> GetPlayerHandAsync(PlayerSessionRequest request)
        {
            var cards = await _handCardRepository.FilterAsync(c => c.SessionId == request.SessionId && c.PlayerId == request.PlayerId);

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
