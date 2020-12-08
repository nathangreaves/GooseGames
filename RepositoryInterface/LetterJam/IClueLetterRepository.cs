using Entities.LetterJam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.LetterJam
{
    public interface IClueLetterRepository : ICommonRepository<ClueLetter>
    {
        Task<IList<ClueLetter>> GetNonPlayerCharacterLettersUsedForClueAsync(Guid clueId);
    }
}
