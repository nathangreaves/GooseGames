using Entities.Fuji;
using Entities.Global.Enums;
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
        private readonly Global.SessionService _sessionService;
        private readonly IHandCardRepository _handCardRepository;
        private readonly IPlayerInformationRepository _playerRepository;
        private readonly RequestLogger<HandService> _logger;

        public HandService(Global.SessionService sessionService,
            IHandCardRepository handCardRepository,
            IPlayerInformationRepository playerRepository,
            RequestLogger<HandService> logger)
        {
            _sessionService = sessionService;
            _handCardRepository = handCardRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerHand>> GetPlayerHandAsync(PlayerSessionRequest request)
        {
            var gameId = await _sessionService.GetGameIdAsync(request.SessionId, GameEnum.FujiFlush);
            if (gameId == null)
            {
                return GenericResponse<PlayerHand>.Error("Unable to find game");
            }
            var player = await _playerRepository.GetPlayerInformationFromPlayerIdAndGameId(request.PlayerId, gameId.Value);
            var playedCardId = player.PlayedCardId;

            var cards = await _handCardRepository.FilterAsync(c => c.GameId == gameId.Value && c.PlayerId == request.PlayerId && (playedCardId == null || playedCardId != c.Id));

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
