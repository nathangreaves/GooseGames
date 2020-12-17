using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class GameConfiguration
    {
        public int NumberOfPlayers { get; set; }
        public int NumberOfRedClues { get; set; }
        public int NumberOfGreenClues { get; set; }
        public int NumberOfLockedGreenClues { get; set; }
        public int NumberOfRedCluesPerPlayer
        {
            get
            {
                return NumberOfRedClues / NumberOfPlayers;
            }

        }
        public List<NonPlayerCharacterConfiguration> NonPlayerCharacters { get; set; }
    }

    public class NonPlayerCharacterConfiguration
    {
        public int NumberOfLetters { get; set; }
    }

    public static class GameConfigurationService
    {
        private static Dictionary<int, GameConfiguration> s_Config = new Dictionary<int, GameConfiguration>(new List<GameConfiguration>
        {
            new GameConfiguration
            {
                NumberOfPlayers = 2,
                NumberOfRedClues = 6,
                NumberOfGreenClues = 2,
                NumberOfLockedGreenClues = 7,
                NonPlayerCharacters = new List<NonPlayerCharacterConfiguration>
                {
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 7
                    },
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 8
                    },
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 9
                    },
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 10
                    }
                }
            },
            new GameConfiguration
            {
                NumberOfPlayers = 3,
                NumberOfRedClues = 6,
                NumberOfGreenClues = 2,
                NumberOfLockedGreenClues = 6,
                NonPlayerCharacters = new List<NonPlayerCharacterConfiguration>
                {
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 7
                    },
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 8
                    },
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 9
                    }
                }
            },
            new GameConfiguration
            {
                NumberOfPlayers = 4,
                NumberOfRedClues = 4,
                NumberOfGreenClues = 6,
                NumberOfLockedGreenClues = 3,
                NonPlayerCharacters = new List<NonPlayerCharacterConfiguration>
                {
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 7
                    },
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 8
                    }
                }
            },
            new GameConfiguration
            {
                NumberOfPlayers = 5,
                NumberOfRedClues = 5,
                NumberOfGreenClues = 5,
                NumberOfLockedGreenClues = 2,
                NonPlayerCharacters = new List<NonPlayerCharacterConfiguration>
                {
                    new NonPlayerCharacterConfiguration
                    {
                        NumberOfLetters = 7
                    }
                }
            },
            new GameConfiguration
            {
                NumberOfPlayers = 6,
                NumberOfRedClues = 6,
                NumberOfGreenClues = 4,
                NumberOfLockedGreenClues = 1,
                NonPlayerCharacters = new List<NonPlayerCharacterConfiguration>()
            }
        }.ToDictionary(g => g.NumberOfPlayers, g => g));

        public static GameConfiguration GetForPlayerCount(int playerCount)
        {
            return s_Config[playerCount];
        }
    }
}
