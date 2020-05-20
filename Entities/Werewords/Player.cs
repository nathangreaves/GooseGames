using Entities.Common;
using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords
{
    public class Player : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
        public int PlayerNumber { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }

        private Guid _status;

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
    }
}
