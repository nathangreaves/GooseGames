using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Avalon
{
    public static class GameConfigurationService
    {
        private static Dictionary<int, GameConfiguration> s_Instances = new Dictionary<int, GameConfiguration>
        {
            { 5, new GameConfiguration
                {
                    NumberOfPlayers = 5,
                    NumberOfEvil = 2
                }
            },
            { 6, new GameConfiguration
                {
                    NumberOfPlayers = 6,
                    NumberOfEvil = 2
                }
            },
            { 7, new GameConfiguration
                {
                    NumberOfPlayers = 7,
                    NumberOfEvil = 3
                }
            },
            { 8, new GameConfiguration
                {
                    NumberOfPlayers = 8,
                    NumberOfEvil = 3
                }
            },
            { 9, new GameConfiguration
                {
                    NumberOfPlayers = 9,
                    NumberOfEvil = 3
                }
            },
            { 10, new GameConfiguration
                {
                    NumberOfPlayers = 10,
                    NumberOfEvil = 4
                }
            },
            { 11, new GameConfiguration
                {
                    NumberOfPlayers = 11,
                    NumberOfEvil = 4
                }
            },
            { 12, new GameConfiguration
                {
                    NumberOfPlayers = 12,
                    NumberOfEvil = 4
                }
            }
        };

        public static GameConfiguration Get(int numberOfPlayers)
        {
            return s_Instances[numberOfPlayers];
        }
    }

    public class GameConfiguration
    {
        public int NumberOfPlayers { get; set; }
        public int NumberOfEvil { get; set; }
    }
}
