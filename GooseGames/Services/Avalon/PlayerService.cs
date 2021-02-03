using GooseGames.Services.Global;
using Models.Avalon;
using Models.Avalon.Roles;
using Models.Avalon.Roles.Types;
using Models.Requests;
using Models.Responses;
using Models.Responses.Avalon;
using RepositoryInterface.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace GooseGames.Services.Avalon
{
    public class PlayerService
    {
        private readonly Global.PlayerService _playerService;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IPlayerIntelRepository _playerIntelRepository;

        public PlayerService(Global.PlayerService playerService,
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            IPlayerIntelRepository playerIntelRepository)
        {
            _playerService = playerService;
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _playerIntelRepository = playerIntelRepository;
        }

        public async Task<GenericResponse<IEnumerable<PlayerResponse>>> GetPlayerAsync(PlayerSessionGameRequest request)
        {
            var game = await _gameRepository.GetAsync(request.GameId);

            var playerRequestIds = new List<Guid>
            {
                request.PlayerId
            };
            if (game.GodPlayerId == request.PlayerId)
            {
                playerRequestIds = (await _playerService.GetForSessionAsync(request.SessionId)).Select(x => x.Id).Where(x => x != request.PlayerId).ToList();
            }

            var playerStates = await _playerStateRepository.GetForPlayerIds(playerRequestIds);
            var playerIntel = await _playerIntelRepository.FilterAsync(p => playerRequestIds.Contains(p.PlayerId));
            Dictionary<Guid, List<Entities.Avalon.PlayerIntel>> playerIntelDictionary = new Dictionary<Guid, List<Entities.Avalon.PlayerIntel>>();
            foreach (var intel in playerIntel)
            {
                if (playerIntelDictionary.ContainsKey(intel.PlayerId))
                {
                    playerIntelDictionary[intel.PlayerId].Add(intel);
                }
                else
                {
                    playerIntelDictionary.Add(intel.PlayerId, new List<Entities.Avalon.PlayerIntel> 
                    { 
                        intel
                    });
                }
            }
            var seatNumbers = await _playerService.GetPlayerNumbersAsync(playerRequestIds);
            return GenericResponse<IEnumerable<PlayerResponse>>.Ok(playerStates.Select(p => 
            {
                var role = AvalonRoleKey.GetRole(p.GameRole.RoleEnum);
                return new PlayerResponse
                {
                    PlayerId = p.PlayerId,
                    Role = new RoleResponse 
                    {
                        Good = role is GoodRoleBase,
                        RoleEnum = role.RoleEnum,
                        RoleWeight = 0
                    },
                    SeatNumber = seatNumbers[p.PlayerId],
                    PlayerIntel = (playerIntelDictionary.ContainsKey(p.PlayerId) ? playerIntelDictionary[p.PlayerId] : new List<Entities.Avalon.PlayerIntel>()).Select(pI =>
                    {
                        return new PlayerIntel
                        {
                            PlayerId = p.PlayerId,
                            IntelNumber = pI.IntelNumber,
                            IntelPlayerId = pI.IntelPlayerId,
                            IntelType = pI.IntelType,
                            RoleKnowsYou = pI.RoleKnowsYou
                        };
                    })
                };
            }).OrderBy(x => x.SeatNumber));
        }
    }
}
