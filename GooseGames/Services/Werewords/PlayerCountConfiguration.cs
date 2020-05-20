using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords
{
    public class PlayerCountConfiguration
    {

        private static readonly Dictionary<int, Dictionary<SecretRolesEnum, int>> s_PlayerCountConfig = new Dictionary<int, Dictionary<SecretRolesEnum, int>>
        {
            { 4, new Dictionary<SecretRolesEnum, int>
                {
                    { SecretRolesEnum.Werewolf, 1 },
                    { SecretRolesEnum.Seer, 1 },
                    { SecretRolesEnum.Villager, 2 }
                }
            },
            { 5, new Dictionary<SecretRolesEnum, int>
                {
                    { SecretRolesEnum.Werewolf, 1 },
                    { SecretRolesEnum.Seer, 1 },
                    { SecretRolesEnum.Villager, 3 }
                }
            },
            { 6, new Dictionary<SecretRolesEnum, int>
                {
                    { SecretRolesEnum.Werewolf, 1 },
                    { SecretRolesEnum.Seer, 1 },
                    { SecretRolesEnum.Villager, 4 }
                }
            },
            { 7, new Dictionary<SecretRolesEnum, int>
                {
                    { SecretRolesEnum.Werewolf, 2 },
                    { SecretRolesEnum.Seer, 1 },
                    { SecretRolesEnum.Villager, 4 }
                }
            },
            { 8, new Dictionary<SecretRolesEnum, int>
               {
                    { SecretRolesEnum.Werewolf, 2 },
                    { SecretRolesEnum.Seer, 1 },
                    { SecretRolesEnum.Villager, 5 }
                }
            },
            { 9, new Dictionary<SecretRolesEnum, int>
                {
                    { SecretRolesEnum.Werewolf, 2 },
                    { SecretRolesEnum.Seer, 1 },
                    { SecretRolesEnum.Villager, 6 }
                }
            },
            { 10, new Dictionary<SecretRolesEnum, int>
                {
                    { SecretRolesEnum.Werewolf, 2 },
                    { SecretRolesEnum.Seer, 1 },
                    { SecretRolesEnum.Villager, 7 }
                }
            }
        };

        public static Stack<SecretRolesEnum> GetShuffledSecretRolesList(int playerCount)
        {
            var random = new Random();
            Dictionary<int, SecretRolesEnum> allRoles = new Dictionary<int, SecretRolesEnum>();

            var config = s_PlayerCountConfig[playerCount];

            foreach (var configItem in config)
            {
                for (int i = 0; i < configItem.Value; i++)
                {
                    allRoles.Add(random.Next(), configItem.Key);
                }
            }


            return new Stack<SecretRolesEnum>(allRoles.OrderBy(x => x.Key).Select(x => x.Value));
        }
    }
}
