using Entities.Werewords.Game;
using RepositoryInterface.Werewords.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryRepository.Werewords.Game
{
    public class WerewordsGameRepository : IWerewordsGameRepository
    {
        private static readonly Dictionary<Guid, WerewordsGame> s_Games = new Dictionary<Guid, WerewordsGame>();

        public Task<Guid> Create(string password) 
        {
            var newId = Guid.NewGuid();

            s_Games.Add(newId, new WerewordsGame
            {
                Id = newId,
                Password = password,
                NumberOfPlayers = 1
            });

            return Task.FromResult(newId);
        }

        public Task<bool> ExistsWithPassword(string password)
        {
            return Task.FromResult(s_Games.Any(g => g.Value.Password == password));
        }

        public Task<WerewordsGame> AddPlayer(string password)
        {
            var game = s_Games.Values.Single(g => g.Password == password);

            game.NumberOfPlayers++;

            return Task.FromResult(game);
        }
    }
}
