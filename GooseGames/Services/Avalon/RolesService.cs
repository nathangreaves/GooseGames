using Enums.Avalon;
using GooseGames.Hubs;
using Models.Avalon.Roles;
using Models.Avalon.Roles.Types;
using Models.Requests;
using Models.Requests.Avalon;
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
        private readonly AvalonHubContext _avalonHubContext;
        private static readonly Random s_Random = new Random();

        public RolesService(Global.PlayerService playerService,
            IGameRoleRepository gameRoleRepository,
            AvalonHubContext avalonHubContext)
        {
            _playerService = playerService;
            _gameRoleRepository = gameRoleRepository;
            _avalonHubContext = avalonHubContext;
        }

        public async Task<GenericResponse<IEnumerable<RoleResponse>>> AllRolesAsync(PlayerSessionRequest request)
        {
            var numberOfPlayers = await _playerService.GetCountForSessionAsync(request.SessionId);

            var array = Enum.GetValues(typeof(GameRoleEnum)) as IEnumerable<GameRoleEnum>;
            var roles = array.ToList().Where(x => !(x == GameRoleEnum.LoyalServantOfArthur) && !(x == GameRoleEnum.MinionOfMordred)).Select(x => AvalonRoleKey.GetRole(x)).Where(x => x != null);

            return GenericResponse<IEnumerable<RoleResponse>>.Ok(roles.Select(x => new RoleResponse
            {
                RoleEnum = x.RoleEnum,
                Good = x is GoodRoleBase,
                ViableForDrunkToMimic = x.ViableForDrunkToMimic,
                ViableForMyopiaInfo = x.ViableForMyopiaInfo
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
                    Good = role is GoodRoleBase,
                    ViableForDrunkToMimic = role.ViableForDrunkToMimic,
                    ViableForMyopiaInfo = role.ViableForMyopiaInfo
                };
            }).OrderByDescending(x => x.Good).ThenBy(x => x.RoleEnum));
        }

        public async Task<GenericResponse<int>> GetWeightAsync(StartSessionRequest request)
        {
            var numberOfPlayers = await _playerService.GetCountForSessionAsync(request.SessionId);
            if (request.GodMode)
            {
                numberOfPlayers -= 1;
            }
            int weight = GetWeight(request, numberOfPlayers);

            await _avalonHubContext.SendWeightAsync(request.SessionId, weight);

            return GenericResponse<int>.Ok(weight);
        }

        private static int GetWeight(StartSessionRequest request, int numberOfPlayers)
        {
            var allRoles = request.Roles.Select(x => AvalonRoleKey.GetRole(x));

            var goodRoles = allRoles.Count(x => x is GoodRoleBase);
            var expectedGoodRoles = numberOfPlayers - GameConfigurationService.Get(numberOfPlayers).NumberOfEvil;

            int weight = allRoles.Sum(x => x.GetRoleWeightInPlayAgnostic(numberOfPlayers, expectedGoodRoles, allRoles));
            if (goodRoles > expectedGoodRoles)
            {
                //var numberOfExtraRoles = goodRoles - expectedGoodRoles;
                //weight += (int)Math.Ceiling((double)numberOfExtraRoles / 2); ;

                weight += 1;

                List<int> weights = new List<int>();
                for (int i = 0; i < 100; i++)
                {
                    var rolesInPlay = allRoles.OrderBy(x => s_Random.Next()).Take(expectedGoodRoles).ToList() as IEnumerable<AvalonRoleBase>;
                    weights.Add(rolesInPlay.Sum(x => x.GetRoleWeight(numberOfPlayers, rolesInPlay, allRoles)));
                }
                weight += (short)Math.Round((weights.Sum() / 100.0));
            }
            else
            {
                weight += allRoles.Sum(x => x.GetRoleWeight(numberOfPlayers, allRoles, allRoles));
            }

            return weight;
        }
    }
}
