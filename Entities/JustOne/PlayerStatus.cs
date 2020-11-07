using Entities.Common;
using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne
{
    public class PlayerStatus : IHasGuidId, IHasCreatedUtc
    {
        private Guid _status;

        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid Status 
        { 
            get 
            { 
                return _status; 
            } 
            set 
            { 
                _status = value; 
                Description = PlayerStatusEnum.GetDescription(value); 
            } 
        }
        public string Description { get; set; }
        public string ConnectionId { get; set; }
        public PlayerStatus()
        {
            Id = Guid.NewGuid();
        }
    }
}
