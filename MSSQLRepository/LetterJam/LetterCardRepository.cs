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

        public async Task<LetterCard> GetNextNpcCardAsync(Guid npcId, Guid currentLetterId)
        {
            var currentLetterIndex = await GetPropertyAsync(currentLetterId, p => p.LetterIndex);

            var npcLetter = await Context.LetterCards
                .Where(c => c.NonPlayerCharacterId == npcId && c.LetterIndex > currentLetterIndex)
                .OrderBy(x => x.LetterIndex)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return npcLetter;
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
                    npcEntity.NumberOfLettersRemaining -= 1;
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

        public async Task<LetterCard> GetNextUndiscardedCardAsync(Guid gameId)
        {
            var card = await Context.LetterCards
                .Where(x => x.GameId == gameId && x.Discarded == false && x.NonPlayerCharacterId == null && x.PlayerId == null && x.BonusLetter == false)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (card == null)
            {
                var discardedCards = await Context.LetterCards
                .Where(x => x.GameId == gameId && x.Discarded == true)
                .ToListAsync()
                .ConfigureAwait(false);

                foreach (var c in discardedCards)
                {
                    c.Discarded = false;
                    c.BonusLetter = false;
                    c.NonPlayerCharacterId = null;
                    c.PlayerId = null;
                    c.LetterIndex = null;
                    c.PlayerLetterGuess = null;
                }

                await UpdateRangeAsync(discardedCards);

                card = await Context.LetterCards
                .Where(x => x.GameId == gameId && x.Discarded == false && x.NonPlayerCharacterId == null && x.PlayerId == null && x.BonusLetter == false)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            }

            return card;
        }
    }
}
