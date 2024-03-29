﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses
{
    public class PlayerActionResponse
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public int PlayerNumber { get; set; }
        public string Emoji { get; set; }
        public bool HasTakenAction { get; set; }
    }
}
