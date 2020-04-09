using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne.Enums
{
    public enum ResponseStatusEnum
    {
        New = 0,
        AutoInvalid = 1,
        ManualInvalid = 2,
        Valid = 3,
        AutoCorrectActivePlayerResponse = 4,
        CorrectActivePlayerResponse = 4,
        IncorrectActivePlayerResponse = 5,
        PassedActivePlayerResponse = 6
    }
}
