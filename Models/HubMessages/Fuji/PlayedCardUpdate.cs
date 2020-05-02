using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HubMessages.Fuji
{
    public class PlayedCardUpdate
    {
        public Guid PlayerId { get; set; }
        public int FaceValue { get; set; }
        public int CombinedValue { get; set; }
    }
}
