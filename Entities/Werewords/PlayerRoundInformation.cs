﻿using Entities.Common;
using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords
{
    public class PlayerRoundInformation : IHasGuidId, IHasCreatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Guid PlayerId { get; set; }
        public Guid RoundId { get; set; }
        public Round Round { get; set; }
        public SecretRolesEnum SecretRole { get; set; }
        public bool IsMayor { get; set; }
        public ICollection<PlayerResponse> Responses { get; set; }

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
