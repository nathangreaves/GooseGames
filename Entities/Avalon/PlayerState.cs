using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Avalon
{
    public class PlayerState : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public Guid AssumedRoleId { get; set; }
        public GameRole AssumedRole { get; set; }
        public Guid ActualRoleId { get; set; }
        public GameRole ActualRole { get; set; }
    }
}
