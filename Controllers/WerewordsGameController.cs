using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GooseGames.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WerewordsGameController : ControllerBase
    {
        [HttpPost]
        public Guid Post(string password)
        {


            return Guid.NewGuid();
        }
    }
}