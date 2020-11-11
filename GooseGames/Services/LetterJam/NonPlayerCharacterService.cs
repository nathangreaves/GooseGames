using GooseGames.Logging;
using Models.Requests;
using Models.Responses;
using Models.Responses.LetterJam;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class NonPlayerCharacterService
    {
        private readonly INonPlayerCharacterRepository _nonPlayerCharacterRepository;
        private readonly RequestLogger<NonPlayerCharacterService> _logger;

        public NonPlayerCharacterService(INonPlayerCharacterRepository nonPlayerCharacterRepository,
            RequestLogger<NonPlayerCharacterService> logger)
        {
            _nonPlayerCharacterRepository = nonPlayerCharacterRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<IEnumerable<NonPlayerCharacterResponse>>> GetNonPlayerCharactersAsync(PlayerSessionGameRequest request)
        {
            var characters = await _nonPlayerCharacterRepository.FilterAsync(nPC => nPC.GameId == request.GameId);

            return GenericResponse<IEnumerable<NonPlayerCharacterResponse>>.Ok(characters.Select(c => new NonPlayerCharacterResponse
            {
                NonPlayerCharacterId = c.Id,
                Name = c.Name,
                Emoji = c.Emoji,
                PlayerNumber = c.PlayerNumber
            }));
        }
    }
}
