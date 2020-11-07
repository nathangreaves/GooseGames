using Models.Enums.Werewords;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.Werewords
{
    public class OtherPlayerSecretRoleResponse
    {
        public string PlayerName { get; set; }
        public Guid PlayerId { get; set; }
        public SecretRole SecretRole { get; set; }
    }
}
