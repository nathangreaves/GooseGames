using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Models
{
    public class CreateWerewordsGameResponse
    {
        public Guid? GameId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
