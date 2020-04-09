﻿using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne
{
    public class Player : IHasGuidId, IHasCreatedUtc
    {
        public Player()
        {
            Id = Guid.NewGuid();
        }
        public DateTime CreatedUtc { get; set; }
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
        public int PlayerNumber { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public PlayerStatus PlayerStatus { get; set; }
        public ICollection<Response> Responses { get; set; }
    }
}