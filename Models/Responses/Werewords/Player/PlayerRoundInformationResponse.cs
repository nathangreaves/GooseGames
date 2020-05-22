using Models.Enums.Werewords;
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
        public IEnumerable<PlayerResponse> Responses { get; set; }
        public SecretRole? SecretRole { get; set; }
    }
}
