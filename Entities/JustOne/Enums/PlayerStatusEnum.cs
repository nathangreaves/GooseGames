using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne.Enums
{
    public static class PlayerStatusEnum
    {
        public static readonly Guid New = new Guid("4d90d127-a8f4-4c91-9ac3-228e7310c154");
        public static readonly Guid InLobby = new Guid("5786f3a4-d429-4f55-b061-167dc416bd9a");
        public static readonly Guid RoundWaiting = new Guid("7d7c73b0-9718-459c-bc1f-cb9e17bbdf51");
        public static readonly Guid PassivePlayerClue = new Guid("20bf1daf-e809-4d71-a302-c29755ee97a4");
        public static readonly Guid PassivePlayerWaitingForClues = new Guid("9d311821-83ca-4650-9b7a-96f1aa6afed3");
        public static readonly Guid PassivePlayerClueVote = new Guid("64019614-2155-4427-8cd3-56e2cad9d4bb");
        public static readonly Guid PassivePlayerWaitingForClueVotes = new Guid("2a90fbf6-1558-4231-a3ee-e332165b7e43");
        public static readonly Guid PassivePlayerWaitingForActivePlayer = new Guid("8cb25d76-4e82-492a-a4df-7b33cefae549");
        public static readonly Guid PassivePlayerOutcome = new Guid("54ede5f2-cece-4b03-af7f-3dcfe6253807");
        public static readonly Guid PassivePlayerOutcomeVote = new Guid("ed5d9c11-7363-4cc9-a079-4f70d60f7e0d");
        public static readonly Guid PassivePlayerWaitingForOutcomeVotes = new Guid("0d620fb5-532e-4fc7-9cb1-eaadc03ec971");

        public static readonly Guid ActivePlayerWaitingForClues = new Guid("7c2277d7-d73a-4760-8686-631adc82f449");
        public static readonly Guid ActivePlayerWaitingForVotes = new Guid("6f1902c0-22eb-4bd8-a36a-152ce4d772e7");
        public static readonly Guid ActivePlayerGuess = new Guid("0131e28c-89a5-4140-a4fc-357a081bb9b6");
        public static readonly Guid ActivePlayerWaitingForOutcomeVotes = new Guid("cc955a44-fa89-442f-ac80-f092f376a589");
        public static readonly Guid ActivePlayerOutcome = new Guid("ffcc0414-f358-4314-beb8-0b5aa97d8aed");

        private static readonly Dictionary<Guid, string> s_Instances = new Dictionary<Guid, string>
        {
            { New, nameof(New) },
            { InLobby, nameof(InLobby) },
            { RoundWaiting, nameof(RoundWaiting) },
            { PassivePlayerClue, nameof(PassivePlayerClue) },
            { PassivePlayerWaitingForClues, nameof(PassivePlayerWaitingForClues) },
            { PassivePlayerClueVote, nameof(PassivePlayerClueVote) },
            { PassivePlayerWaitingForClueVotes, nameof(PassivePlayerWaitingForClueVotes) },
            { PassivePlayerWaitingForActivePlayer, nameof(PassivePlayerWaitingForActivePlayer) },
            { PassivePlayerOutcome, nameof(PassivePlayerOutcome) },
            { PassivePlayerOutcomeVote, nameof(PassivePlayerOutcomeVote) },
            { PassivePlayerWaitingForOutcomeVotes, nameof(PassivePlayerWaitingForOutcomeVotes) },
            { ActivePlayerWaitingForClues, nameof(ActivePlayerWaitingForClues) },
            { ActivePlayerWaitingForVotes, nameof(ActivePlayerWaitingForVotes) },
            { ActivePlayerGuess, nameof(ActivePlayerGuess) },
            { ActivePlayerWaitingForOutcomeVotes, nameof(ActivePlayerWaitingForOutcomeVotes) },
            { ActivePlayerOutcome, nameof(ActivePlayerOutcome) }
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
    }
}
