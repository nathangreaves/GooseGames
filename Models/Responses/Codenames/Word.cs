using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Codenames
{
    public class CodenamesWord
    {
        public Guid Id { get; set; }
        public string Word { get; set; }
        public WordType WordType { get; set; }
        public bool Revealed { get; set; }
    }
}
