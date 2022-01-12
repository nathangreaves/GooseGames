using Entities.Avalon;
using GooseGames.Hubs;
using GooseGames.Services.Global;
using Models.Avalon;
using Models.Avalon.Roles;
using Models.Avalon.Roles.Types;
using Models.Requests;
using Models.Requests.Avalon;
using Models.Responses;
using Models.Responses.PlayerStatus;
using RepositoryInterface.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Avalon
{
    public class LobbyService
    {
        private readonly SessionService _sessionService;
        private readonly Global.PlayerService _globalPlayerService;
        private readonly GlobalPlayerStatusService _globalPlayerStatusService;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IPlayerIntelRepository _playerIntelRepository;
        private readonly IGameRoleRepository _gameRoleRepository;
        private readonly AvalonHubContext _avalonHubContext;

        public LobbyService(
            Global.SessionService sessionService,
            Global.PlayerService globalPlayerService,
            GlobalPlayerStatusService globalPlayerStatusService,
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            IPlayerIntelRepository playerIntelRepository,
            IGameRoleRepository gameRoleRepository,
            AvalonHubContext avalonHubContext)
        {
            _sessionService = sessionService;
            _globalPlayerService = globalPlayerService;
            _globalPlayerStatusService = globalPlayerStatusService;
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _playerIntelRepository = playerIntelRepository;
            _gameRoleRepository = gameRoleRepository;
            _avalonHubContext = avalonHubContext;
        }

        public async Task<GenericResponseBase> StartSessionAsync(StartSessionRequest request)
        {
            //Start the session and prepare the intel accordingly.
            var response = await _sessionService.ValidateSessionToStartAsync(request, 6, 13);
            if (!response.Success)
            {
                return response;
            }
            var players = await _globalPlayerService.GetForSessionAsync(request.SessionId);
            var numberOfActualPlayers = players.Count - 1;

            var roles = request.Roles.Select(x => AvalonRoleKey.GetRole(x)).ToList();
            var gameConfiguration = GameConfigurationService.Get(numberOfActualPlayers);
            //if (roles.Count > numberOfActualPlayers)
            //{
            //    return GenericResponseBase.Error("Too many roles selected for number of players");
            //}
            int numberOfEvilRoles = roles.Where(x => x is EvilRoleBase).Count();
            if (numberOfEvilRoles > gameConfiguration.NumberOfEvil)
            {
                return GenericResponseBase.Error("Too many evil roles selected");
            }
            int numberOfGoodRoles = roles.Where(x => !(x is EvilRoleBase)).Count();
            //if (numberOfGoodRoles > gameConfiguration.NumberOfPlayers - gameConfiguration.NumberOfEvil)
            //{
            //    return GenericResponseBase.Error("Too many good roles selected");
            //}
            var missingEvilPlayers = gameConfiguration.NumberOfEvil - numberOfEvilRoles;
            var missingGoodPlayers = (gameConfiguration.NumberOfPlayers - gameConfiguration.NumberOfEvil) - numberOfGoodRoles;
            for (int i = 0; i < missingEvilPlayers; i++)
            {
                roles.Add(new MinionOfMordred());
            }
            for (int i = 0; i < missingGoodPlayers; i++)
            {
                roles.Add(new LoyalServantOfArthur());
            }

            await _sessionService.StartSessionAsync(request.SessionId);

            var game = new Game 
            { 
                Id = Guid.NewGuid(),
                GodPlayerId = request.PlayerId,
                NumberOfPlayers = numberOfActualPlayers,
                SessionId = request.SessionId                
            };
            await _gameRepository.InsertAsync(game);
            await _sessionService.SetGameSessionIdentifierAsync(request.SessionId, Entities.Global.Enums.GameEnum.Avalon, game.Id);

            var gameRoles = roles.Select(x => new GameRole
            {
                GameId = game.Id,
                Id = Guid.NewGuid(),
                RoleEnum = x.RoleEnum                
            }).ToList();
            await _gameRoleRepository.InsertRangeAsync(gameRoles);

            var random = new Random();

            var allRoles = gameRoles.Select(x => AvalonRoleKey.GetRole(x.RoleEnum)).ToList();
            var goodRoles = allRoles.Where(x => x is GoodRoleBase).OrderBy(x => random.Next()).ToList();
            var evilRoles = allRoles.Where(x => x is EvilRoleBase);

            var inPlayRoles = evilRoles.Concat(goodRoles.Take(gameConfiguration.NumberOfPlayers - gameConfiguration.NumberOfEvil)).ToList();
            var inPlayRoleEnums = inPlayRoles.Select(x => x.RoleEnum).ToList();
            var inPlayGameRoles = gameRoles.Where(x => inPlayRoleEnums.Contains(x.RoleEnum));

            var rolesQueue = new Queue<GameRole>(inPlayGameRoles);
            
            var playerStates = players
                .Where(x => x.Id != request.PlayerId) //Remove when God no longer required
                .Select(x => 
                {
                    var role = rolesQueue.Dequeue();

                    var assumedRole = AvalonRoleKey.GetRole(role.RoleEnum).GetAssumedRole(allRoles);
                    var assumedRoleGameRole = gameRoles.First(x => x.RoleEnum == assumedRole);

                    return new PlayerState
                    {
                        PlayerId = x.Id,
                        GameId = game.Id,
                        SessionId = game.SessionId,
                        ActualRole = role,
                        ActualRoleId = role.Id,
                        AssumedRoleId = assumedRoleGameRole.Id,
                        AssumedRole = assumedRoleGameRole
                    };
                }).ToList();



            await _playerStateRepository.InsertRangeAsync(playerStates);

            var seatNumbers = await _globalPlayerService.GetPlayerNumbersAsync(playerStates.Select(x => x.PlayerId));

            var playersWithRole = playerStates.Select(x => new Player
            {
                PlayerId = x.PlayerId,
                ActualRoleEnum = x.ActualRole.RoleEnum,
                AssumedRoleEnum = x.AssumedRole.RoleEnum,
                ActualRole = AvalonRoleKey.GetRole(x.ActualRole.RoleEnum),
                AssumedRole = AvalonRoleKey.GetRole(x.AssumedRole.RoleEnum),
                SeatNumber = seatNumbers[x.PlayerId]
            }).ToList();

            var intel = new List<Entities.Avalon.PlayerIntel>();
            foreach (var playerWithRole in playersWithRole)
            {
                intel.AddRange(playerWithRole.ActualRole.GeneratePlayerIntel(playerWithRole, playersWithRole, allRoles).Select(p => {
                    return new Entities.Avalon.PlayerIntel 
                    { 
                        GameId = game.Id,
                        PlayerId = p.PlayerId,
                        IntelNumber = p.IntelNumber,
                        IntelPlayerId = p.IntelPlayerId,
                        IntelType = p.IntelType,
                        RoleKnowsYou = p.RoleKnowsYou                        
                    };
                }));
            }
            await _playerIntelRepository.InsertRangeAsync(intel);

            await _avalonHubContext.SendBeginSessionAsync(request.SessionId, game.Id);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponse<PlayerStatusGameValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionPossibleGameRequest request, Enums.Avalon.PlayerStatusEnum requestedStatus)
        {
            var globalResponse = await _globalPlayerStatusService.ValidatePlayerStatusAsync(request, Entities.Global.Enums.GameEnum.Avalon);
            if (!globalResponse.Success)
            {
                return GenericResponse<PlayerStatusGameValidationResponse>.Error(globalResponse.ErrorCode);
            }
            var globalPlayerStatus = globalResponse.Data;

            Enums.Avalon.PlayerStatusEnum playerStatus;
            Guid? gameId = request.GameId;
            if (globalPlayerStatus == Entities.Global.Enums.PlayerStatusEnum.Lobby)
            {
                playerStatus = Enums.Avalon.PlayerStatusEnum.InLobby;
            }
            else if (globalPlayerStatus == Entities.Global.Enums.PlayerStatusEnum.Ready)
            {
                playerStatus = Enums.Avalon.PlayerStatusEnum.InLobby;
            }
            else
            {
                if (!gameId.HasValue)
                {
                    gameId = await _sessionService.GetGameIdAsync(request.SessionId, Entities.Global.Enums.GameEnum.Avalon);
                }
                if (gameId == null)
                {
                    return GenericResponse<PlayerStatusGameValidationResponse>.Error("Could not find game");
                }
                var player = await _playerStateRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId && p.GameId == gameId.Value);
                if (player == null)
                {
                    if (request.PlayerId != await _gameRepository.GetPropertyAsync(gameId.Value, x => x.GodPlayerId))
                    {
                        return GenericResponse<PlayerStatusGameValidationResponse>.Error("Could not find avalon player");
                    }
                }

                playerStatus = Enums.Avalon.PlayerStatusEnum.InGame;
            }

            var response = new PlayerStatusGameValidationResponse
            {
                RequiredStatus = Enum.GetName(typeof(Enums.Avalon.PlayerStatusEnum), playerStatus),
                StatusCorrect = requestedStatus == playerStatus,
                GameId = gameId
            };
            return GenericResponse<PlayerStatusGameValidationResponse>.Ok(response);
        }
    }
}
