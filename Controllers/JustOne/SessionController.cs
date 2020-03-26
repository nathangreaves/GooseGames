using Microsoft.AspNetCore.Mvc;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Services.JustOne;

namespace GooseGames.Controllers.JustOne
{
    [ApiController]
    [Route("[controller]")]
    public class JustOneSessionController : ControllerBase
    {
        private readonly SessionService _werewordsGameRepository;

        public JustOneSessionController(SessionService werewordsGameRepository)
        {
            _werewordsGameRepository = werewordsGameRepository;
        }

        [HttpPost]
        public async Task<GenericResponse<NewSessionResponse>> PostAsync(NewSessionRequest request)
        {
            try
            {
                return await _werewordsGameRepository.CreateSessionAsync(request);
            }
            catch (Exception e)
            {
                //TODO: Log exception
                return NewResponse.Error<NewSessionResponse>("Unknown Error");
            }
        }

        [HttpPatch]
        public async Task<GenericResponse<JoinSessionResponse>> PatchAsync(JoinSessionRequest request)
        {
            try
            {
                return await _werewordsGameRepository.JoinSessionAsync(request);

               
            }
            catch (Exception e)
            {
                //TODO: Log exception
                return NewResponse.Error<JoinSessionResponse>("Unknown Error");
            }
        }
    }
}
