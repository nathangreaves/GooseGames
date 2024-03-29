﻿using Entities.Global;
using Entities.Global.Enums;
using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services;
using GooseGames.Services.Global;
using Microsoft.Extensions.Caching.Memory;
using Models.Enum;
using Models.Enums;
using Models.Enums.Werewords;
using Models.Requests;
using Models.Requests.Werewords;
using Models.Responses;
using Models.Responses.Werewords;
using Models.Responses.Werewords.Player;
using Models.Responses.Werewords.Round;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords
{
    public class RoundService
    {
        private readonly IRoundRepository _roundRepository;
        private readonly SessionService _sessionService;
        private readonly Global.PlayerService _playerService;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly IPlayerResponseRepository _playerResponseRepository;
        private readonly IPlayerVoteRepository _playerVoteRepository;
        private readonly PlayerStatusService _playerStatusService;
        private readonly WerewordsHubContext _werewordsHubContext;
        private readonly RequestLogger<RoundService> _logger;
        private readonly IMemoryCache _memoryCache;
        private static readonly Random s_Random = new Random();

        private const int DefaultRoundLength = 4;

        public RoundService(IRoundRepository roundRepository,
            SessionService sessionService,
            Global.PlayerService playerService,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            IPlayerResponseRepository playerResponseRepository,
            IPlayerVoteRepository playerVoteRepository,
            PlayerStatusService playerStatusService,
            WerewordsHubContext werewordsHubContext,
            RequestLogger<RoundService> logger,
            IMemoryCache memoryCache)
        {
            _roundRepository = roundRepository;
            _sessionService = sessionService;
            _playerService = playerService;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _playerResponseRepository = playerResponseRepository;
            _playerVoteRepository = playerVoteRepository;
            _playerStatusService = playerStatusService;
            _werewordsHubContext = werewordsHubContext;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        internal async Task CreateNewRoundAsync(Guid sessionId)
        {
            var players = await _playerService.GetForSessionAsync(sessionId);

            var mayor = players[s_Random.Next(players.Count)];
            var otherPlayers = players.Where(p => p.Id != mayor.Id);

            var round = new Round
            {
                MayorId = mayor.Id,
                SessionId = sessionId,
                RoundDurationMinutes = DefaultRoundLength,
                Status = RoundStatusEnum.NightSecretRole
            };

            await _roundRepository.InsertAsync(round);

            await _sessionService.SetGameSessionIdentifierAsync(sessionId, GameEnum.Werewords, round.Id);

            var secretRoles = PlayerCountConfiguration.GetShuffledSecretRolesList(players.Count);

            foreach (var player in players)
            {
                await _playerRoundInformationRepository.InsertAsync(new PlayerRoundInformation
                {
                    PlayerId = player.Id,
                    Round = round,
                    SecretRole = secretRoles.Pop(),
                    IsMayor = player.Id == mayor.Id
                });
            }

            await _playerStatusService.UpdateAllPlayersForSessionAsync(round.Id, Entities.Werewords.Enums.PlayerStatusEnum.NightRevealSecretRole);

            await _werewordsHubContext.SendSecretRoleAsync(sessionId);
        }

        internal GenericResponse<IEnumerable<string>> GetWordChoiceAsync(PlayerSessionRequest request)
        {
            string cacheKey = $"{request.SessionId}_{request.PlayerId}";
            var cachedWords = _memoryCache.Get<List<string>>(cacheKey);
            if (cachedWords != null)
            {
                return GenericResponse<IEnumerable<string>>.Ok(cachedWords);
            }

            var words = StaticWordsList.GetWords(3, new[] { WordListEnum.JustOne, WordListEnum.Codenames, WordListEnum.CodenamesDuet });

            _memoryCache.Set(cacheKey, words, DateTimeOffset.UtcNow.AddMinutes(2));

            return GenericResponse<IEnumerable<string>>.Ok(words);
        }

        internal async Task<GenericResponseBase> SubmitPlayerResponseAsync(SubmitPlayerResponseRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponseBase.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.GameSessionId.Value);

            if (round == null)
            {
                return GenericResponseBase.Error("Unable to fetch current round");
            }
            if (!round.MayorId.HasValue)
            {
                return GenericResponseBase.Error("Round does not have a mayor!");
            }

            if (round.MayorId.Value != request.PlayerId)
            {
                return GenericResponseBase.Error("Player other than mayor gave a player response");
            }

            var playerInformations = await _playerRoundInformationRepository.GetForRoundAsync(round.Id);

            var respondingPlayer = playerInformations.First(p => p.PlayerId == request.RespondingPlayerId);

            var response = new Entities.Werewords.PlayerResponse
            {
                PlayerRoundInformation = respondingPlayer,
                ResponseType = (PlayerResponseTypeEnum)(int)request.ResponseType
            };
            await _playerResponseRepository.InsertAsync(response);

            await _werewordsHubContext.SendPlayerResponseAsync(session.Id, new Models.Responses.Werewords.Player.PlayerResponse
            {
                PlayerId = request.RespondingPlayerId,
                ResponseType = request.ResponseType
            });

            if (request.ResponseType == PlayerResponseType.Correct)
            {
                await _playerStatusService.UpdateAllPlayersForSessionAsync(round.Id, Entities.Werewords.Enums.PlayerStatusEnum.DayVotingOnSeer);

                round.Status = RoundStatusEnum.DayVoteSeer;
                round.VoteStartedUtc = DateTime.UtcNow.AddSeconds(1);
                round.VoteDurationSeconds = 30;

                await _roundRepository.UpdateAsync(round);
                await _werewordsHubContext.SendVoteSeerAsync(session.Id, round.VoteStartedUtc.AddSeconds(round.VoteDurationSeconds), playerInformations.Where(p => p.SecretRole == SecretRolesEnum.Werewolf).Select(p => p.PlayerId), round.SecretWord);
            }
            else if (TimerExpired(round))
            {
                await SetRoundToVotingOnWerewolvesAsync(round);
            }
            else 
            {
                if (request.ResponseType == PlayerResponseType.SoClose || request.ResponseType == PlayerResponseType.WayOff)
                {
                    round.SoCloseSpent = round.SoCloseSpent || request.ResponseType == PlayerResponseType.SoClose;
                    round.WayOffSpent = round.WayOffSpent || request.ResponseType == PlayerResponseType.WayOff;

                    await _roundRepository.UpdateAsync(round);
                }

                var mayorPlayerId = playerInformations.First(p => p.IsMayor).PlayerId;

                var currentActivePlayer = respondingPlayer;
                currentActivePlayer.Status = Entities.Werewords.Enums.PlayerStatusEnum.DayPassive;
                var nextActivePlayerId = await _playerService.GetNextActivePlayerAsync(request.SessionId, respondingPlayer.PlayerId, p => p.Id == mayorPlayerId);

                var nextActivePlayer = playerInformations.First(p => p.PlayerId == nextActivePlayerId);
                nextActivePlayer.Status = Entities.Werewords.Enums.PlayerStatusEnum.DayActive;

                await _playerRoundInformationRepository.UpdateAsync(currentActivePlayer);
                await _playerRoundInformationRepository.UpdateAsync(nextActivePlayer);

                await _werewordsHubContext.SendActivePlayerAsync(session.Id, nextActivePlayer.PlayerId);
            }            

            return GenericResponseBase.Ok();
        }

        private async Task SetRoundToVotingOnWerewolvesAsync(Round round)
        {
            var setRoundToVotingOnWerewolvesRequestKey = $"{round.Id}_VoW";

            var request = _memoryCache.Get<string>(setRoundToVotingOnWerewolvesRequestKey);

            if (request != null)
            {
                return;
            }

            _memoryCache.Set(setRoundToVotingOnWerewolvesRequestKey, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.AddSeconds(30));

            await _playerStatusService.UpdateAllPlayersForSessionAsync(round.Id, Entities.Werewords.Enums.PlayerStatusEnum.DayVotingOnWerewolves);

            round.Status = RoundStatusEnum.DayVoteWerewolves;
            round.VoteStartedUtc = DateTime.UtcNow.AddSeconds(1);
            round.VoteDurationSeconds = 60;

            await _roundRepository.UpdateAsync(round);
            await _werewordsHubContext.SendVoteWerewolvesAsync(round.SessionId, round.VoteStartedUtc.AddSeconds(round.VoteDurationSeconds), round.SecretWord);
        }

        internal async Task<GenericResponseBase> SubmitVoteAsync(SubmitVoteRequest request, PlayerVoteTypeEnum voteType)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponseBase.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.GameSessionId.Value);

            if (round == null)
            {
                return GenericResponseBase.Error("Unable to fetch current round");
            }

            if (VoteTimerExpired(round))
            {
                return GenericResponseBase.Error("Unable to cast vote after timer has run out");
            }

            await _playerVoteRepository.UpsertVoteAsync(new PlayerVote
            {
                PlayerId = request.PlayerId,
                VotedPlayerId = request.NominatedPlayerId,
                RoundId = round.Id,
                VoteType = voteType
            });

            return GenericResponseBase.Ok();
        }

        private bool VoteTimerExpired(Round round)
        {
            if (round.VoteStartedUtc != default)
            {
                return DateTime.UtcNow > round.VoteStartedUtc.AddSeconds(round.VoteDurationSeconds);
            }
            return false;
        }

        private bool TimerExpired(Round round)
        {
            if (round.RoundStartedUtc != default)
            {
                return DateTime.UtcNow > round.RoundStartedUtc.AddMinutes(round.RoundDurationMinutes);
            }
            return false;
        }


        internal async Task<GenericResponseBase> PostWordAsync(WordChoiceRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponseBase.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.GameSessionId.Value);

            if (round == null)
            {
                return GenericResponseBase.Error("Unable to fetch current round");
            }
            if (!round.MayorId.HasValue)
            {
                return GenericResponseBase.Error("Round does not have a mayor!");
            }

            if (round.MayorId.Value != request.PlayerId)
            {
                return GenericResponseBase.Error("Player other than mayor gave a secret word");
            }

            round.SecretWord = request.Word;
            round.Status = RoundStatusEnum.NightRevealSecretWord;
            
            await _roundRepository.UpdateAsync(round);

            await _playerStatusService.ConditionallyUpdateAllPlayersForSessionAsync(round.Id, Entities.Werewords.Enums.PlayerStatusEnum.NightWaitingForMayor, Entities.Werewords.Enums.PlayerStatusEnum.NightSecretWord);
            await _playerStatusService.UpdatePlayerToStatusAsync(request.PlayerId, round.Id, Entities.Werewords.Enums.PlayerStatusEnum.NightSecretWord);

            await _werewordsHubContext.SendSecretWordAsync(session.Id);

            return GenericResponseBase.Ok();
        }


        internal async Task<GenericResponse<SecretWordResponse>> GetSecretWordAsync(PlayerSessionRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<SecretWordResponse>.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponse<SecretWordResponse>.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.GameSessionId.Value);

            if (round == null)
            {
                return GenericResponse<SecretWordResponse>.Error("Unable to fetch current round");
            }
            if (!round.MayorId.HasValue)
            {
                return GenericResponse<SecretWordResponse>.Error("Round does not have a mayor!");
            }
            var players = await _playerRoundInformationRepository.GetForRoundAsync(round.Id);
            var currentPlayer = players.First(p => p.PlayerId == request.PlayerId);
            var mayor = players.First(p => p.PlayerId == round.MayorId.Value);
            var mayorPlayer = await _playerService.GetAsync(mayor.PlayerId);

            return GenericResponse<SecretWordResponse>.Ok(new SecretWordResponse
            {
                SecretRole = (SecretRole)(int)currentPlayer.SecretRole,
                SecretWord = currentPlayer.SecretRole != SecretRolesEnum.Villager || currentPlayer.IsMayor ? round.SecretWord : null,
                MayorName = mayorPlayer.Name,
                MayorPlayerId = mayor.PlayerId,
                MayorEmoji = mayorPlayer.Emoji
            });
        }

        internal async Task<GenericResponse<DayResponse>> GetDayAsync(PlayerSessionRequest request, bool checkTimers = true)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<DayResponse>.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponse<DayResponse>.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.GameSessionId.Value);

            if (round == null)
            {
                return GenericResponse<DayResponse>.Error("Unable to fetch current round");
            }
            if (!round.MayorId.HasValue)
            {
                return GenericResponse<DayResponse>.Error("Round does not have a mayor!");
            }
            var players = await _playerRoundInformationRepository.GetForRoundAsync(round.Id);
            var currentPlayer = players.First(p => p.PlayerId == request.PlayerId);
            var mayor = players.First(p => p.IsMayor);

            var playersDictionary = await _playerService.GetPlayersAsync(players.Select(p => p.PlayerId));

            var isVote = round.Status == RoundStatusEnum.DayVoteSeer || round.Status == RoundStatusEnum.DayVoteWerewolves;

            if (checkTimers)
            {
                if (isVote && VoteTimerExpired(round))
                {
                    await CompleteRoundAsync(round, players);
                    return await GetDayAsync(request, false);
                }
                else if (!isVote && TimerExpired(round) && round.Status != RoundStatusEnum.Complete)
                {
                    await SetRoundToVotingOnWerewolvesAsync(round);
                    return await GetDayAsync(request, false);
                } 
            }

            var mayorPlayer = playersDictionary[mayor.PlayerId];

            return GenericResponse<DayResponse>.Ok(new DayResponse 
            {
                RoundId = round.Id,
                MayorName = mayorPlayer.Name,
                MayorEmoji = mayorPlayer.Emoji,
                MayorPlayerId = mayorPlayer.Id,
                SecretRole = (SecretRole)(int)currentPlayer.SecretRole,
                SecretWord = currentPlayer.SecretRole != SecretRolesEnum.Villager || currentPlayer.IsMayor || isVote ? round.SecretWord : null,
                IsActive = currentPlayer.Status == Entities.Werewords.Enums.PlayerStatusEnum.DayActive,
                EndTime = round.RoundStartedUtc != default ? DateTime.SpecifyKind(round.RoundStartedUtc, DateTimeKind.Utc).AddMinutes(round.RoundDurationMinutes) : (DateTime?)null,
                VoteEndTime = round.VoteStartedUtc != default ? DateTime.SpecifyKind(round.VoteStartedUtc, DateTimeKind.Utc).AddSeconds(round.VoteDurationSeconds) : (DateTime?)null,
                SoCloseSpent = round.SoCloseSpent,
                WayOffSpent = round.WayOffSpent,
                Players = players.OrderBy(p => playersDictionary[p.PlayerId].PlayerNumber).Select(p => {
                    var player = playersDictionary[p.PlayerId];
                    return new PlayerRoundInformationResponse
                    {
                        Id = p.PlayerId,
                        Name = player.Name,
                        Emoji = player.Emoji,
                        Active = p.Status == Entities.Werewords.Enums.PlayerStatusEnum.DayActive,
                        IsMayor = p.IsMayor,
                        SecretRole = round.Status == RoundStatusEnum.DayVoteSeer && p.SecretRole == SecretRolesEnum.Werewolf ? (SecretRole)(int)p.SecretRole : (SecretRole?)null,
                        Responses = p.Responses.OrderBy(r => r.CreatedUtc).Select(r => new Models.Responses.Werewords.Player.PlayerResponse
                        {
                            ResponseType = (PlayerResponseType)(int)r.ResponseType
                        })
                    };
                })
            });
        }

        internal async Task<GenericResponse<RoundOutcomeResponse>> GetOutcomeAsync(PlayerSessionRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<RoundOutcomeResponse>.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponse<RoundOutcomeResponse>.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.GameSessionId.Value);

            if (round == null)
            {
                return GenericResponse<RoundOutcomeResponse>.Error("Unable to fetch current round");
            }
            var players = await _playerRoundInformationRepository.GetForRoundAsync(round.Id);
            var currentPlayer = players.First(p => p.PlayerId == request.PlayerId);

            List<Guid> mostVotedPlayers = new List<Guid>();
            if (round.Outcome == RoundOutcomeEnum.VillagersVotedWerewolf || round.Outcome == RoundOutcomeEnum.WerewolvesVotedSeer)
            {                
                mostVotedPlayers = await GetMostVotedPlayerIdsAsync(round);
            }

            var playersDictionary = await _playerService.GetPlayersAsync(players.Select(p => p.PlayerId));

            return GenericResponse<RoundOutcomeResponse>.Ok(new RoundOutcomeResponse 
            {
                SecretWord = round.SecretWord,
                RoundOutcome = (RoundOutcome)(int)round.Outcome,
                Players = players.Select(p =>
                {
                    var player = playersDictionary[p.PlayerId];
                    return new RoundOutcomePlayerResponse
                    {
                        Id = p.PlayerId,
                        IsMayor = p.IsMayor,
                        Name = player.Name,
                        Emoji= player.Emoji,
                        SecretRole = (SecretRole)(int)p.SecretRole,
                        WasVoted = mostVotedPlayers.Contains(p.PlayerId)
                    };
                })
            });
        }

        private async Task CompleteRoundAsync(Round round, IEnumerable<PlayerRoundInformation> players)
        {
            var completeRoundRequestKey = $"{round.Id}";

            var request = _memoryCache.Get<string>(completeRoundRequestKey);

            if (request != null)
            {
                return;
            }

            _memoryCache.Set(completeRoundRequestKey, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.AddSeconds(30));

            var previousStatus = round.Status;
            round.Status = RoundStatusEnum.Complete;
            IEnumerable<Guid> allWithHighestVotes = await GetMostVotedPlayerIdsAsync(round);

            if (previousStatus == RoundStatusEnum.DayVoteSeer)
            {
                var seer = players.First(p => p.SecretRole == SecretRolesEnum.Seer);
                if (allWithHighestVotes.Any(x => x == seer.PlayerId))
                {
                    round.Outcome = RoundOutcomeEnum.WerewolvesVotedSeer;
                }
                else
                {
                    round.Outcome = RoundOutcomeEnum.WerewolvesVotedWrong;
                }
            }
            else if (previousStatus == RoundStatusEnum.DayVoteWerewolves)
            {
                var werewolves = players.Where(p => p.SecretRole == SecretRolesEnum.Werewolf).Select(p => p.PlayerId).ToList();
                if (allWithHighestVotes.Any(x => werewolves.Contains(x)))
                {
                    round.Outcome = RoundOutcomeEnum.VillagersVotedWerewolf;
                }
                else
                {
                    round.Outcome = RoundOutcomeEnum.VillagersVotedWrong;
                }
            }

            await _playerStatusService.UpdateAllPlayersForSessionAsync(round.Id, Entities.Werewords.Enums.PlayerStatusEnum.DayOutcome);

            await _roundRepository.UpdateAsync(round);

            await _sessionService.SetToLobbyAsync(round.SessionId);

            await _werewordsHubContext.SendRoundOutcomeAsync(round.SessionId);
        }

        private async Task<List<Guid>> GetMostVotedPlayerIdsAsync(Round round)
        {
            var votes = await _playerVoteRepository.FilterAsync(p => p.RoundId == round.Id);
            var groupedVotes = votes.GroupBy(x => x.VotedPlayerId);
            var highestVotes = groupedVotes.OrderBy(g => g.Count()).LastOrDefault();

            var allWithHighestVotes = groupedVotes
                .Where(x => x.Count() == highestVotes.Count())
                .Select(x => x.Key);
            return allWithHighestVotes.ToList();
        }

        internal async Task<GenericResponseBase> StartAsync(PlayerSessionRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponseBase.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponseBase.Error("Session does not have current round");
            }

            var round = await _roundRepository.GetAsync(session.GameSessionId.Value);

            if (round == null)
            {
                return GenericResponseBase.Error("Unable to fetch current round");
            }
            if (!round.MayorId.HasValue)
            {
                return GenericResponseBase.Error("Round does not have a mayor!");
            }

            round.RoundStartedUtc = DateTime.UtcNow.AddSeconds(1);

            await _roundRepository.UpdateAsync(round);

            await _werewordsHubContext.SendStartTimerAsync(session.Id, DateTime.SpecifyKind(round.RoundStartedUtc, DateTimeKind.Utc).AddMinutes(round.RoundDurationMinutes));

            return GenericResponseBase.Ok();
        }
    }
}
