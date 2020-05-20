using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Werewords
{
    public class WordChoiceRequest : PlayerSessionRequest
    {
        public string Word { get; set; }
    }
}
