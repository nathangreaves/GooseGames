using Entities.JustOne;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostGreRepository.JustOne
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
    }
}
