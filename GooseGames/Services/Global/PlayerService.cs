using Entities.Global;
using Entities.Global.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.Requests;
using Models.Requests.PlayerDetails;
using Models.Responses;
using Models.Responses.PlayerDetails;
using RepositoryInterface.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Global
{
    public class PlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly SessionService _sessionRepository;
        private readonly GlobalHubContext _globalHubContext;
        private readonly RequestLogger<PlayerService> _logger;

        private static readonly Random s_Random = new Random();

        private static readonly List<string> emojis = new List<string>{
    "😄", "😃", "😀", "😊", "☺", "😉", "😍", "😘", "😚", "😗", "😙", "😜", "😝", "😛", "😳", "😁", "😔", "😌", "😒", "😞", "😣", "😢", "😂", "😭", "😪", "😥", "😰", "😅", "😓", "😩", "😫", "😨", "😱", "😠", "😡", "😤", "😖", "😆", "😋", "😷", "😎", "😴", "😵", "😲", "😟", "😦", "😧", "😈", "👿", "😮", "😬", "😐", "😕", "😯", "😶", "😇", "😏", "😑", "👲", "👳", "👮", "👷", "💂", "👶", "👦", "👧", "👨", "👩", "👴", "👵", "👱", "👼", "👸", "😺", "😸", "😻", "😽", "😼", "🙀", "😿", "😹", "😾", "👹", "👺", "🙈", "🙉", "🙊", "💀", "👽", "💩", "🔥", "✨", "🌟", "💫", "💥", "💢", "💦", "💧", "💤", "💨", "👂", "👀", "👃", "👅", "👄", "👍", "👎", "👌", "👊", "✊", "✌", "👋", "✋", "👐", "👆", "👇", "👉", "👈", "🙌", "🙏", "☝", "👏", "💪", "🚶", "🏃", "💃", "👫", "👪", "👬", "👭", "💏", "💑", "👯", "🙆", "🙅", "💁", "🙋", "💆", "💇", "💅", "👰", "🙎", "🙍", "🙇", "🎩", "👑", "👒", "👟", "👞", "👡", "👠", "👢", "👕", "👔", "👚", "👗", "🎽", "👖", "👘", "👙", "💼", "👜", "👝", "👛", "👓", "🎀", "🌂", "💄", "💛", "💙", "💜", "💚", "❤", "💔", "💗", "💓", "💕", "💖", "💞", "💘", "💌", "💋", "💍", "💎", "👤", "👥", "💬", "👣", "💭", "🐶", "🐺", "🐱", "🐭", "🐹", "🐰", "🐸", "🐯", "🐨", "🐻", "🐷", "🐽", "🐮", "🐗", "🐵", "🐒", "🐴", "🐑", "🐘", "🐼", "🐧", "🐦", "🐤", "🐥", "🐣", "🐔", "🐍", "🐢", "🐛", "🐝", "🐜", "🐞", "🐌", "🐙", "🐚", "🐠", "🐟", "🐬", "🐳", "🐋", "🐄", "🐏", "🐀", "🐃", "🐅", "🐇", "🐉", "🐎", "🐐", "🐓", "🐕", "🐖", "🐁", "🐂", "🐲", "🐡", "🐊", "🐫", "🐪", "🐆", "🐈", "🐩", "🐾", "💐", "🌸", "🌷", "🍀", "🌹", "🌻", "🌺", "🍁", "🍃", "🍂", "🌿", "🌾", "🍄", "🌵", "🌴", "🌲", "🌳", "🌰", "🌱", "🌼", "🌐", "🌞", "🌝", "🌚", "🌑", "🌒", "🌓", "🌔", "🌕", "🌖", "🌗", "🌘", "🌜", "🌛", "🌙", "🌍", "🌎", "🌏", "🌋", "🌌", "🌠", "⭐", "☀", "⛅", "☁", "⚡", "☔", "❄", "⛄", "🌀", "🌁", "🌈", "🌊", "🎍", "💝", "🎎", "🎒", "🎓", "🎏", "🎆", "🎇", "🎐", "🎑", "🎃", "👻", "🎅", "🎄", "🎁", "🎋", "🎉", "🎊", "🎈", "🎌", "🔮", "🎥", "📷", "📹", "📼", "💿", "📀", "💽", "💾", "💻", "📱", "☎", "📞", "📟", "📠", "📡", "📺", "📻", "🔊", "🔉", "🔈", "🔇", "🔔", "🔕", "📢", "📣", "⏳", "⌛", "⏰", "⌚", "🔓", "🔒", "🔏", "🔐", "🔑", "🔎", "💡", "🔦", "🔆", "🔅", "🔌", "🔋", "🔍", "🛁", "🛀", "🚿", "🚽", "🔧", "🔩", "🔨", "🚪", "🚬", "💣", "🔫", "🔪", "💊", "💉", "💰", "💴", "💵", "💷", "💶", "💳", "💸", "📲", "📧", "📥", "📤", "✉", "📩", "📨", "📯", "📫", "📪", "📬", "📭", "📮", "📦", "📝", "📄", "📃", "📑", "📊", "📈", "📉", "📜", "📋", "📅", "📆", "📇", "📁", "📂", "✂", "📌", "📎", "✒", "✏", "📏", "📐", "📕", "📗", "📘", "📙", "📓", "📔", "📒", "📚", "📖", "🔖", "📛", "🔬", "🔭", "📰", "🎨", "🎬", "🎤", "🎧", "🎼", "🎵", "🎶", "🎹", "🎻", "🎺", "🎷", "🎸", "👾", "🎮", "🃏", "🎴", "🀄", "🎲", "🎯", "🏈", "🏀", "⚽", "⚾", "🎾", "🎱", "🏉", "🎳", "⛳", "🚵", "🚴", "🏁", "🏇", "🏆", "🎿", "🏂", "🏊", "🏄", "🎣", "☕", "🍵", "🍶", "🍼", "🍺", "🍻", "🍸", "🍹", "🍷", "🍴", "🍕", "🍔", "🍟", "🍗", "🍖", "🍝", "🍛", "🍤", "🍱", "🍣", "🍥", "🍙", "🍘", "🍚", "🍜", "🍲", "🍢", "🍡", "🍳", "🍞", "🍩", "🍮", "🍦", "🍨", "🍧", "🎂", "🍰", "🍪", "🍫", "🍬", "🍭", "🍯", "🍎", "🍏", "🍊", "🍋", "🍒", "🍇", "🍉", "🍓", "🍑", "🍈", "🍌", "🍐", "🍍", "🍠", "🍆", "🍅", "🌽", "🏠", "🏡", "🏫", "🏢", "🏣", "🏥", "🏦", "🏪", "🏩", "🏨", "💒", "⛪", "🏬", "🏤", "🌇", "🌆", "🏯", "🏰", "⛺", "🏭", "🗼", "🗾", "🗻", "🌄", "🌅", "🌃", "🗽", "🌉", "🎠", "🎡", "⛲", "🎢", "🚢", "⛵", "🚤", "🚣", "⚓", "🚀", "✈", "💺", "🚁", "🚂", "🚊", "🚉", "🚞", "🚆", "🚄", "🚅", "🚈", "🚇", "🚝", "🚋", "🚃", "🚎", "🚌", "🚍", "🚙", "🚘", "🚗", "🚕", "🚖", "🚛", "🚚", "🚨", "🚓", "🚔", "🚒", "🚑", "🚐", "🚲", "🚡", "🚟", "🚠", "🚜", "💈", "🚏", "🎫", "🚦", "🚥", "⚠", "🚧", "🔰", "⛽", "🏮", "🎰", "♨", "🗿", "🎪", "🎭", "📍", "🚩", "⬆", "⬇", "⬅", "➡", "🔠", "🔡", "🔤", "↗", "↖", "↘", "↙", "↔", "↕", "🔄", "◀", "▶", "🔼", "🔽", "↩", "↪", "ℹ", "⏪", "⏩", "⏫", "⏬", "⤵", "⤴", "🆗", "🔀", "🔁", "🔂", "🆕", "🆙", "🆒", "🆓", "🆖", "📶", "🎦", "🈁", "🈯", "🈳", "🈵", "🈴", "🈲", "🉐", "🈹", "🈺", "🈶", "🈚", "🚻", "🚹", "🚺", "🚼", "🚾", "🚰", "🚮", "🅿", "♿", "🚭", "🈷", "🈸", "🈂", "Ⓜ", "🛂", "🛄", "🛅", "🛃", "🉑", "㊙", "㊗", "🆑", "🆘", "🆔", "🚫", "🔞", "📵", "🚯", "🚱", "🚳", "🚷", "🚸", "⛔", "✳", "❇", "❎", "✅", "✴", "💟", "🆚", "📳", "📴", "🅰", "🅱", "🆎", "🅾", "💠", "➿", "♻", "♈", "♉", "♊", "♋", "♌", "♍", "♎", "♏", "♐", "♑", "♒", "♓", "⛎", "🔯", "🏧", "💹", "💲", "💱", "©", "®", "™", "〽", "〰", "🔝", "🔚", "🔙", "🔛", "🔜", "❌", "⭕", "❗", "❓", "❕", "❔", "🔃", "🕛", "🕧", "🕐", "🕜", "🕑", "🕝", "🕒", "🕞", "🕓", "🕟", "🕔", "🕠", "🕕", "🕖", "🕗", "🕘", "🕙", "🕚", "🕡", "🕢", "🕣", "🕤", "🕥", "🕦", "✖", "➕", "➖", "➗", "♠", "♥", "♣", "♦", "💮", "💯", "✔", "☑", "🔘", "🔗", "➰", "🔱", "🔲", "🔳", "◼", "◻", "◾", "◽", "▪", "▫", "🔺", "⬜", "⬛", "⚫", "⚪", "🔴", "🔵", "🔻", "🔶", "🔷", "🔸", "🔹"
};
        public PlayerService(
            IPlayerRepository playerRepository,
            SessionService sessionRepository,
            GlobalHubContext globalHubContext,
            RequestLogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _sessionRepository = sessionRepository;
            _globalHubContext = globalHubContext;
            _logger = logger;
        }


        internal async Task<Player> GetAsync(Guid playerId)
        {
            return await _playerRepository.GetAsync(playerId);
        }

        internal async Task<GenericResponseBase> DeletePlayerAsync(DeletePlayerRequest request)
        {
            _logger.LogTrace("Deleting Player", request);

            var requestingPlayer = await _playerRepository.GetAsync(request.SessionMasterId);
            if (requestingPlayer == null)
            {
                return GenericResponseBase.Error("Who even are you?");
            }

            var playerToDelete = await _playerRepository.GetAsync(request.PlayerToDeleteId);
            if (playerToDelete == null)
            {
                _logger.LogWarning("Asked to delete player that didn't exist");
                await _globalHubContext.SendPlayerRemoved(requestingPlayer.SessionId.Value, request.PlayerToDeleteId);
                return GenericResponseBase.Ok();
            }

            var isSessionMaster = await ValidateSessionMasterAsync(playerToDelete.SessionId.Value, request.SessionMasterId);
            if (isSessionMaster)
            {
                playerToDelete.SessionId = null;
                playerToDelete.PlayerNumber = 0;
                playerToDelete.Status = PlayerStatusEnum.Abandoned;
                await _playerRepository.UpdateAsync(playerToDelete);

                await _globalHubContext.SendPlayerRemoved(requestingPlayer.SessionId.Value, playerToDelete.Id);
            }

            _logger.LogTrace("Deleted Player");
            return GenericResponseBase.Ok();
        }


        public async Task<GenericResponse<GetPlayerDetailsResponse>> GetPlayerDetailsAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Starting fetch of player details");

            _logger.LogTrace("Fetching session");
            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {
                _logger.LogWarning("Unable to find session.");
                return GenericResponse<GetPlayerDetailsResponse>.Error("Unable to find session.");
            }

            var players = await _playerRepository.FilterAsync(player => player.SessionId == request.SessionId);
            if (!players.Any(p => p.Id == request.PlayerId))
            {
                _logger.LogWarning("Player did not exist on session");
                return GenericResponse<GetPlayerDetailsResponse>.Error("Player does not exist on session");
            }

            var masterPlayerId = session.SessionMasterId;

            var sessionMaster = players.FirstOrDefault(p => p.Id == masterPlayerId);

            var response = new GetPlayerDetailsResponse
            {
                SessionMaster = request.PlayerId == masterPlayerId,
                SessionMasterName = sessionMaster?.Name,
                SessionMasterPlayerNumber = sessionMaster?.PlayerNumber,
                Password = session.Password,
                RandomEmoji = emojis[s_Random.Next(0, emojis.Count)],
                Players = players.OrderBy(p => p.PlayerNumber == 0 ? int.MaxValue : p.PlayerNumber).Select(p => new PlayerDetailsResponse
                {
                    Id = p.Id,
                    IsSessionMaster = p.Id == masterPlayerId,
                    PlayerName = p.Name,
                    Emoji = p.Emoji,
                    Ready = p.Status == PlayerStatusEnum.Ready
                })
            };

            return GenericResponse<GetPlayerDetailsResponse>.Ok(response);
        }

        internal async Task<Dictionary<Guid, Player>> GetPlayersAsync(IEnumerable<Guid> playerIds)
        {
            var hashset = new HashSet<Guid>(playerIds);

            return (await _playerRepository.FilterAsync(p => hashset.Contains(p.Id))).ToDictionary(p => p.Id, p => p);
        }

        internal async Task<List<Player>> GetForSessionAsync(Guid sessionId)
        {
            return await _playerRepository.FilterAsync(p => p.SessionId == sessionId);
        }

        internal async Task<GenericResponseBase> UpdatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
        {
            _logger.LogTrace("Starting update of player details");

            var validationResult = await ValidatePlayerDetailsAsync(request);
            if (validationResult != null && !validationResult.Success)
            {
                return validationResult;
            }

            var player = await _playerRepository.GetAsync(request.PlayerId);
            if (player == null)
            {
                _logger.LogWarning("Unable to find player.");

                return GenericResponseBase.Error("Unable to find player.");
            }

            player.Name = request.PlayerName;
            player.Emoji = request.Emoji;

            if (player.PlayerNumber == 0)
            {
                _logger.LogTrace("Getting player number");
                int nextPlayerNumber = await _playerRepository.GetNextPlayerNumberAsync(request.SessionId);
                _logger.LogTrace($"Player number = {nextPlayerNumber}");
                player.PlayerNumber = nextPlayerNumber;
            }

            player.Status = PlayerStatusEnum.Ready;

            _logger.LogTrace("Updating player details");
            await _playerRepository.UpdateAsync(player);

            await SendPlayerDetailsUpdate(player);

            _logger.LogTrace("Finished updating player details");

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> UnreadyPlayerAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Starting update of player details");

            if (!(await ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.Lobby)))
            {
                _logger.LogWarning("Unable to find session. Either it is not in lobby or doesn't exist.");

                return GenericResponseBase.Error("Unable to find session. Either it started without you or doesn't exist");
            }


            var player = await _playerRepository.GetAsync(request.PlayerId);
            if (player == null)
            {
                _logger.LogWarning("Unable to find player.");

                return GenericResponseBase.Error("Unable to find player.");
            }

            player.Status = PlayerStatusEnum.Lobby;

            _logger.LogTrace("Updating player details");
            await _playerRepository.UpdateAsync(player);

            await SendPlayerDetailsUpdate(player);

            _logger.LogTrace("Finished updating player details");

            return GenericResponseBase.Ok();
        }

        private async Task SendPlayerDetailsUpdate(Player player)
        {
            _logger.LogTrace("Sending update to clients");
            await _globalHubContext.SendPlayerDetailsUpdated(player.SessionId.Value, new PlayerDetailsResponse
            {
                Id = player.Id,
                PlayerName = player.Name,
                Emoji = player.Emoji,
                IsSessionMaster = false,
                Ready = player.Status == PlayerStatusEnum.Ready
            });
        }

        private async Task<GenericResponseBase> ValidatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
        {
            _logger.LogTrace("Starting validation of player details");

            if (string.IsNullOrWhiteSpace(request.PlayerName))
            {
                _logger.LogWarning("Empty player name provided");
                return GenericResponseBase.Error("Please enter your name.");
            }

            if (request.PlayerName.Length > 20)
            {
                _logger.LogWarning("Player name too long");
                return GenericResponseBase.Error("Please enter a player name that is 20 characters or fewer");
            }

            if (!(await ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.Lobby)))
            {
                _logger.LogWarning("Unable to find session. Either it is not in lobby or doesn't exist.");

                return GenericResponseBase.Error("Unable to find session. Either it started without you or doesn't exist");
            }

            return null;
        }

        private async Task<bool> ValidateSessionStatusAsync(Guid sessionId, SessionStatusEnum sessionStatus)
        {
            return await _sessionRepository.ValidateSessionStatusAsync(sessionId, sessionStatus);
        }

        internal async Task<bool> ValidateSessionMasterAsync(Guid sessionId, Guid sessionMasterId)
        {
            return await _sessionRepository.ValidateSessionMasterAsync(sessionId, sessionMasterId);
        }

        internal async Task<string> GetPlayerNameAsync(Guid playerId)
        {
            return await _playerRepository.GetPropertyAsync(playerId, p => p.Name);
        }

        internal async Task<Dictionary<Guid, string>> GetPlayerNamesAsync(IEnumerable<Guid> playerIds)
        {
            return await _playerRepository.GetPropertyDictionaryAsync(playerIds, p => p.Name);
        }

        internal async Task<int> GetPlayerNumberAsync(Guid playerId)
        {
            return await _playerRepository.GetPropertyAsync(playerId, p => p.PlayerNumber);
        }

        internal async Task<Dictionary<Guid, int>> GetPlayerNumbersAsync(IEnumerable<Guid> playerIds)
        {
            return await _playerRepository.GetPropertyDictionaryAsync(playerIds, p => p.PlayerNumber);
        }

        internal async Task<Guid> GetNextActivePlayerAsync(Guid sessionId, Guid? currentActivePlayerId, Func<Player, bool> excludePlayer = null)
        {
            var players = await GetForSessionAsync(sessionId);

            return GetNextActivePlayer(players, currentActivePlayerId, excludePlayer);
        }


        internal async Task<GenericResponseBase> SendPlayerToLobbyAsync(PlayerSessionRequest request)
        {
            var player = await _playerRepository.GetAsync(request.PlayerId);
            if (player == null)
            {
                return GenericResponseBase.Error($"Player not found");
            }

            player.Status = PlayerStatusEnum.Ready;

            await _playerRepository.UpdateAsync(player);

            await _globalHubContext.SendPlayerDetailsUpdated(player.SessionId.Value, new PlayerDetailsResponse
            {
                Id = player.Id,
                PlayerName = player.Name,
                Emoji = player.Emoji,
                Ready = true
            });

            return GenericResponseBase.Ok();
        }

        private static Guid GetNextActivePlayer(IEnumerable<Player> players, Guid? currentActivePlayerId, Func<Player, bool> excludePlayer)
        {
            //TODO: Could we not just remove the exluded players from this list rather than using recursion below?
            var orderedPlayerList = players.OrderBy(x => x.PlayerNumber).ToList();

            Player nextActivePlayer = null;
            if (currentActivePlayerId == null)
            {
                nextActivePlayer = orderedPlayerList[new Random().Next(orderedPlayerList.Count)];
            }
            else if (orderedPlayerList.Last().Id == currentActivePlayerId)
            {
                nextActivePlayer = orderedPlayerList.First();
            }
            else
            {
                var currentActivePlayer = orderedPlayerList.First(p => p.Id == currentActivePlayerId);

                var indexOfPrevious = orderedPlayerList.IndexOf(currentActivePlayer);
                nextActivePlayer = orderedPlayerList[indexOfPrevious + 1];
            }

            if (excludePlayer != null && excludePlayer(nextActivePlayer))
            {
                return GetNextActivePlayer(players, nextActivePlayer.Id, excludePlayer);
            }

            return nextActivePlayer.Id;
        }

    }
}
