using Models.Responses.PlayerStatus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.PlayerStatus
{
    public class PlayerStatusGameValidationResponse : PlayerStatusValidationResponse
    {
        public Guid? GameId { get; set; }
    }
}
