using Entities.Werewords;
using Entities.Werewords.Enums;
using Models.Responses;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords.PlayerStatus
{
    public class NightRevealSecretRoleService : PlayerStatusKeyedServiceBase
    {
        private readonly IRoundRepository _round;

        public override Guid PlayerStatus => PlayerStatusEnum.NightRevealSecretRole;

        public NightRevealSecretRoleService(IRoundRepository round, IPlayerRepository playerRepository) : base(playerRepository)
        {
            _round = round;
        }

        protected override async Task<Guid> GetNextStatusAsync(Session session, PlayerRoundInformation playerRoundInformation)
        {
            if (playerRoundInformation.IsMayor)
            {
                return PlayerStatusEnum.NightMayorPickSecretWord;
            }

            var currentRoundStatus = await _round.GetPropertyAsync(session.CurrentRoundId.Value, r => r.Status);

            if (currentRoundStatus == RoundStatusEnum.NightRevealSecretWord)
            {
                return PlayerStatusEnum.NightSecretWord;
            }

            return PlayerStatusEnum.NightWaitingForMayor;
        }
    }
}
