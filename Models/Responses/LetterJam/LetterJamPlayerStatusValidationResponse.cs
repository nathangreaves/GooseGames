using Models.Responses.PlayerStatus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class LetterJamPlayerStatusValidationResponse : PlayerStatusValidationResponse
    {
        public Guid? GameId { get; set; }
    }
}
