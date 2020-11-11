using Entities.LetterJam;
using Microsoft.EntityFrameworkCore;
using MSSQLRepository.Contexts;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.LetterJam
{
    public class LetterCardRepository : CommonRepository<LetterCard>, ILetterCardRepository
    {
        private LetterJamContext Context
        {
            get
            {
                return (LetterJamContext)DbContext;
            }
        }

        public LetterCardRepository(LetterJamContext context)
            : base(context)
        {

        }

        public async Task ReserveLettersForNonPlayerCharacterAsync(NonPlayerCharacter npcEntity)
        {            
            var lettersForGame = await GetUnreservedCardsForGameQueryable(npcEntity.GameId)
                .Take(npcEntity.NumberOfLettersRemaining)
                .ToListAsync()
                .ConfigureAwait(false);

            var letterIndex = 0;
            foreach (var letter in lettersForGame)
            {
                if (letterIndex == 0)
                {
                    npcEntity.CurrentLetterId = letter.Id;                    
                }
                letter.NonPlayerCharacterId = npcEntity.Id;
                letter.LetterIndex = letterIndex;
                letterIndex++;
            }

            await UpdateRangeAsync(lettersForGame).ConfigureAwait(false);

            Context.NonPlayerCharacters.Update(npcEntity);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        private IQueryable<LetterCard> GetUnreservedCardsForGameQueryable(Guid gameId)
        {
            return Context.LetterCards
                            .Where(l => l.GameId == gameId && !l.PlayerId.HasValue && !l.NonPlayerCharacterId.HasValue && !l.BonusLetter).AsQueryable();
        }

        public async Task UnreserveAllCardsForGameAsync(Guid gameId)
        {
            var lettersForGame = await Context.LetterCards
                .Where(l => l.GameId == gameId && l.PlayerId.HasValue)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var l in lettersForGame)
            {
                l.PlayerId = null;
                l.LetterIndex = null;
            }

            await UpdateRangeAsync(lettersForGame)
                .ConfigureAwait(false);
        }
    }
}
