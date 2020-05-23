using Models.Enums.Werewords;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords.Player
{
    public class RoundOutcomePlayerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SecretRole SecretRole { get; set; }
        public bool IsMayor { get; set; }
        public bool WasVoted { get; set; }
    }
}
