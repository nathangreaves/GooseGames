using Entities.LetterJam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.LetterJam
{
    public class ClueLetterRepository : CommonRepository<ClueLetter>, IClueLetterRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public ClueLetterRepository(LetterJamContext context)
            : base(context)
        {

        }

        public async Task<IList<ClueLetter>> GetNonPlayerCharacterLettersUsedForClueAsync(Guid clueId)
        {
            var clueLetters = await Context.ClueLetters.AsQueryable()
                .Include(x => x.LetterCard)
                .Where(x => x.ClueId == clueId && x.NonPlayerCharacterId.HasValue)
                .ToListAsync()
                .ConfigureAwait(false);

            return clueLetters;
        }

        public async Task<Dictionary<Guid, IEnumerable<ClueLetter>>> GetForCluesAsync(IEnumerable<Guid> clueIds)
        {
            var clueIdsList = clueIds.ToList();

            var clueLetters = await Context.ClueLetters
                .Include(x => x.LetterCard)
                .AsQueryable()
                .Where(x => clueIdsList.Contains(x.ClueId))
                .ToListAsync()
                .ConfigureAwait(false);

            return clueLetters
                .GroupBy(x => x.ClueId)
                .ToDictionary(g => g.Key, g => g as IEnumerable<ClueLetter>);
        }
    }
}
