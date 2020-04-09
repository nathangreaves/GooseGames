using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.JustOne.PlayerStatus
{
    public class PlayerStatusValidationResponse
    {
        public bool StatusCorrect { get; set; }
        public string RequiredStatus { get; set; }
    }
}
