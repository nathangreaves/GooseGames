using Entities.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.LetterJam
{
    public interface ILetterCardRepository : ICommonRepository<LetterCard>
    {
        Task ReserveLettersForNonPlayerCharacterAsync(NonPlayerCharacter npcEntity);
        Task UnreserveAllCardsForGameAsync(Guid gameId);
        Task<LetterCard> GetNextNpcCardAsync(Guid npcId, Guid currentLetterId);
        Task<LetterCard> GetNextUndiscardedCardAsync(Guid gameId);
    }
}
