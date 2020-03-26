using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Models
{
    public class JoinWerewordsGameResponse
    {
        public Guid? GameId { get; set; }        
        public int NumberOfPlayers { get; set; }
        public string ErrorMessage { get; set; }
    }
}
