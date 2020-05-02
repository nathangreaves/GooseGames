using Models.Requests;
using Models.Responses;
using Models.Responses.Fuji.Hands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Fuji
{
    public class HandService
    {
        internal Task<GenericResponse<PlayerHand>> GetPlayerHandAsync(PlayerSessionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
