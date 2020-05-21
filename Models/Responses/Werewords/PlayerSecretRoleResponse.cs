using Models.Enums.Werewords;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords
{
    public class PlayerSecretRoleResponse
    {
        public SecretRole SecretRole { get; set; }
        public Guid MayorPlayerId { get; set; }
        public string MayorName { get; set; }
    }
}
