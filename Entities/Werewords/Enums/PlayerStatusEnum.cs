using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords.Enums
{
    public static class PlayerStatusEnum
    {
        public static readonly Guid New = new Guid("4d90d127-a8f4-4c91-9ac3-228e7310c154");
        public static readonly Guid InLobby = new Guid("5786f3a4-d429-4f55-b061-167dc416bd9a");
        public static readonly Guid RoundWaiting = new Guid("7d7c73b0-9718-459c-bc1f-cb9e17bbdf51");
        public static readonly Guid NightRevealSecretRole = new Guid("88df62e6-7b63-4c10-868b-18998f309f87");
        public static readonly Guid NightWaitingForPlayersToCheckRole = new Guid("a1384417-5fbc-4cd8-b423-c7974efa2071");
        public static readonly Guid NightMayorPickSecretWord = new Guid("ec2e65bf-e1a9-4163-9409-38a251d847c3");
        public static readonly Guid NightWaitingForMayor = new Guid("1c228228-ffa4-4cdc-a116-7c403584d577");
        public static readonly Guid NightSecretWord = new Guid("780518c3-a952-4ab3-b89a-474889be3304");
        public static readonly Guid NightWaitingToWake = new Guid("b8bcb932-7cc5-40cd-b49e-ddefa3742659");
        public static readonly Guid DayMayor = new Guid("b3792d08-dcaa-4f58-a6b7-d060d73e315f");
        public static readonly Guid DayPassive = new Guid("2002c22a-9783-4de3-b5af-d11f49d4b675");
        public static readonly Guid DayActive = new Guid("8272a180-00e4-4651-9674-c33a7cfb0cb3");
        public static readonly Guid DayWaitingForVotes = new Guid("68323dca-38fd-482c-a535-41635d4a9e05");
        public static readonly Guid DayVotingOnWerewolves = new Guid("008af81c-59cc-4727-9fd4-08ccdd9eb8b2");
        public static readonly Guid DayVotingOnSeer = new Guid("5281e1ba-1eb6-48c6-8848-b788b7448054");
        public static readonly Guid DayOutcome = new Guid("c12a3f0f-9b30-4675-a2f1-0cba4d64be8c");
        public static readonly Guid WaitingForNextRound = new Guid("3f4b501b-9cb6-4e76-9a19-1b37c87ca136");
        public static readonly Guid Rejoining = new Guid();

        private static readonly Dictionary<Guid, string> s_Instances = new Dictionary<Guid, string>
        {
            { New, nameof(New) },
            { InLobby, nameof(InLobby) },
            { RoundWaiting, nameof(RoundWaiting) },

            { Rejoining, nameof(Rejoining) }
        };
        public static string GetDescription(Guid guid)
        {
            return s_Instances[guid];
        }

        public static string TryGetDescription(Guid guid)
        {
            try
            {
                return GetDescription(guid);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
                
        //29631347-c800-40bd-82f3-3d6aa077660d
        //156df54b-dc50-4856-a0d5-789d1878e02b
        //7ab2f4ed-64d8-4471-909d-6c6e657e56cb
        //76853d01-98c7-4307-a74a-9e6cdfef5e07
        //6557d874-ce4b-4eee-be41-abea0b560c5b
        //8a1bfb19-150a-471c-8bfb-45642113b8b1
    }
}
