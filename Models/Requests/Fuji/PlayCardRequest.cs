﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Fuji
{
    public class PlayCardRequest : PlayerSessionRequest
    {
        public Guid CardId { get; set; }
    }
}
