using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne.RoundStatus
{
    public class RoundServiceProvider
    {
        private readonly Dictionary<RoundStatusEnum, IRoundStatusKeyedService> _services;

        public RoundServiceProvider(IEnumerable<IRoundStatusKeyedService> services)
        {
            _services = services.ToDictionary(service => service.RoundStatus, service => service);
        }

        public IRoundStatusKeyedService GetService(RoundStatusEnum roundStatus)
        {
            return _services[roundStatus];
        }
    }
}
