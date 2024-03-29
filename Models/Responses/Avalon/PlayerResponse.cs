﻿using Enums.Avalon;
using Models.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Avalon
{
    public class PlayerResponse
    {
        public Guid PlayerId { get; set; }
        public int SeatNumber { get; set; }
        public RoleResponse AssumedRole { get; set; }
        public RoleResponse ActualRole { get; set; }
        public IEnumerable<PlayerIntel> PlayerIntel { get; set; }
    }
}
