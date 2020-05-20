using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords.Player
{
    public class PlayerRoundInformationResponse
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public int Ticks { get; set; }
        public int Crosses { get; set; }
        public int QuestionMarks { get; set; }
        public int SoClose { get; set; }
        public int Correct { get; set; }
    }
}
