using Entities.JustOne;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.JustOne
{
    public class ResponseVoteRepository : CommonRepository<ResponseVote>, IResponseVoteRepository
    {
        public JustOneContext Context
        {
            get
            {
                return (JustOneContext)DbContext;
            }
        }

        public ResponseVoteRepository(JustOneContext context) : base(context)
        {

        }

        public async Task<Dictionary<Guid, int>> GetNumberOfVotesPerResponseAsync(IEnumerable<Guid> enumerable)
        {
            var listOfIds = enumerable as IList<Guid> ?? enumerable.ToList();

            var responseDictionary = (await Context.ResponseVotes
                .Where(r => listOfIds.Contains(r.ResponseId))
                .GroupBy(r => r.ResponseId)
                .Select(r => new KeyValuePair<Guid, int>(r.Key, r.Count()))
                .ToListAsync())
                .ToDictionary(r => r.Key, r => r.Value);

            foreach (var responseId in listOfIds)
            {
                if (!responseDictionary.ContainsKey(responseId))
                {
                    responseDictionary.Add(responseId, 0);
                }
            }

            return responseDictionary;
        }


        public async Task DeleteForPlayerAsync(Guid roundId, Guid playerId)
        {
            var responses = await Context.Responses                
                .Where(r => r.RoundId == roundId)
                .SelectMany(r => r.ResponseVotes)
                .Where(r => r.PlayerId == playerId)
                .ToListAsync();

            if (responses != null && responses.Any())
            {
                foreach (var response in responses)
                {
                    await DeleteAsync(response);
                }
            }
        }

        public async Task DeleteActivePlayerResponseVoteForPlayerAsync(Guid roundId, Guid playerId)
        {
            var activePlayerId = await Context.Rounds.Where(r => r.Id == roundId).Select(x => x.ActivePlayerId).SingleAsync();

            var response = await Context.Responses
                .Where(response => response.RoundId == roundId && response.PlayerId == activePlayerId)
                .SelectMany(r => r.ResponseVotes)
                .Where(responseVote => responseVote.PlayerId == playerId)
                .SingleOrDefaultAsync();

            if (response != null)
            {
                await DeleteAsync(response);
            }
        }
    }
}
