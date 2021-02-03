using Enums.Avalon;
using Models.Avalon.Roles;
using Models.Requests;
using Models.Responses;
using Models.Responses.Avalon;
using RepositoryInterface.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Avalon
{
    public class RolesService
    {
        private readonly Global.PlayerService _playerService;
        private readonly IGameRoleRepository _gameRoleRepository;

        public RolesService(Global.PlayerService playerService,
            IGameRoleRepository gameRoleRepository)
        {
            _playerService = playerService;
            _gameRoleRepository = gameRoleRepository;
        }

        public async Task<GenericResponse<IEnumerable<RoleResponse>>> AllRolesAsync(PlayerSessionRequest request)
        {
            var numberOfPlayers = await _playerService.GetCountForSessionAsync(request.SessionId);

            var array = Enum.GetValues(typeof(GameRoleEnum)) as IEnumerable<GameRoleEnum>;
            var roles = array.ToList().Where(x => !(x == GameRoleEnum.LoyalServantOfArthur) && !(x == GameRoleEnum.MinionOfMordred)).Select(x => AvalonRoleKey.GetRole(x)).Where(x => x != null);

            return GenericResponse<IEnumerable<RoleResponse>>.Ok(roles.Select(x => new RoleResponse
            {
                RoleEnum = x.RoleEnum,
                RoleWeight = x.GetRoleWeight(numberOfPlayers),
                Good = x is GoodRoleBase
            }));
        }

        public async Task<GenericResponse<IEnumerable<RoleResponse>>> RolesInGameAsync(PlayerSessionGameRequest request)
        {
            var numberOfPlayers = await _playerService.GetCountForSessionAsync(request.SessionId);

            var roles = await _gameRoleRepository.FilterAsync(x => x.GameId == request.GameId);

            return GenericResponse<IEnumerable<RoleResponse>>.Ok(roles.Select(x => {
                var role = AvalonRoleKey.GetRole(x.RoleEnum);
                return new RoleResponse
                {
                    RoleEnum = role.RoleEnum,
                    RoleWeight = role.GetRoleWeight(numberOfPlayers),
                    Good = role is GoodRoleBase
                };
            }).OrderByDescending(x => x.Good).ThenBy(x => x.RoleEnum));
        }
    }
}
