using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords.PlayerStatus
{
    public class PlayerStatusKeyedServiceProvider
    {
        private readonly Dictionary<Guid, IPlayerStatusKeyedService> _services;

        public PlayerStatusKeyedServiceProvider(IEnumerable<IPlayerStatusKeyedService> services)
        {
            _services = services.ToDictionary(service => service.PlayerStatus, service => service);
        }

        public IPlayerStatusKeyedService GetService(Guid playerStatus)
        {
            return _services[playerStatus];
        }
    }
}
