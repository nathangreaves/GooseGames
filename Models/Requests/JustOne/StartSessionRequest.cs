using Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.JustOne
{
    public class StartSessionRequest : PlayerSessionRequest
    {
        public IEnumerable<WordListEnum> IncludedWordLists { get; set; }
    }
}
