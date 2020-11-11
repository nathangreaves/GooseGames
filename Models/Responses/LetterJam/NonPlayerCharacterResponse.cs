using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class NonPlayerCharacterResponse
    {
        public Guid NonPlayerCharacterId { get; set; }
        public string Name { get; set; }
        public string Emoji { get; set; }
        public int PlayerNumber { get; set; }
    }
}
