using Entities.Common;
using Entities.LetterJam.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class PlayerState : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }

        private PlayerStatusId _status;

        public PlayerStatusId Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                StatusDescription = PlayerStatus.GetDescription(value);
            }
        }

        public string StatusDescription { get; set; }
        public Guid? CurrentLetterId { get; set; }
        public LetterCard CurrentLetter { get; set; }
        public int? CurrentLetterIndex { get; set; }
        public int NumberOfCluesGiven { get; set; }
        public int OriginalWordLength { get; set; }
        public int? FinalWordLength { get; set; }
        public bool? Successful { get; set; }
        public int? Points { get; set; }
    }
}
