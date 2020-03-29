"use strict";
var _a;
Object.defineProperty(exports, "__esModule", { value: true });
var PlayerStatus;
(function (PlayerStatus) {
    PlayerStatus[PlayerStatus["New"] = 0] = "New";
    PlayerStatus[PlayerStatus["InLobby"] = 1] = "InLobby";
    PlayerStatus[PlayerStatus["RoundWaiting"] = 2] = "RoundWaiting";
    PlayerStatus[PlayerStatus["PassivePlayerClue"] = 3] = "PassivePlayerClue";
    PlayerStatus[PlayerStatus["PassivePlayerWaitingForClues"] = 4] = "PassivePlayerWaitingForClues";
    PlayerStatus[PlayerStatus["PassivePlayerClueVote"] = 5] = "PassivePlayerClueVote";
    PlayerStatus[PlayerStatus["PassivePlayerWaitingForClueVotes"] = 6] = "PassivePlayerWaitingForClueVotes";
    PlayerStatus[PlayerStatus["PassivePlayerWaitingForActivePlayer"] = 7] = "PassivePlayerWaitingForActivePlayer";
    PlayerStatus[PlayerStatus["PassivePlayerOutcome"] = 8] = "PassivePlayerOutcome";
    PlayerStatus[PlayerStatus["PassivePlayerOutcomeVote"] = 9] = "PassivePlayerOutcomeVote";
    PlayerStatus[PlayerStatus["ActivePlayerWaitingForClues"] = 10] = "ActivePlayerWaitingForClues";
    PlayerStatus[PlayerStatus["ActivePlayerWaitingForVotes"] = 11] = "ActivePlayerWaitingForVotes";
    PlayerStatus[PlayerStatus["ActivePlayerGuess"] = 12] = "ActivePlayerGuess";
    PlayerStatus[PlayerStatus["ActivePlayerWaitingForOutcomeVotes"] = 13] = "ActivePlayerWaitingForOutcomeVotes";
    PlayerStatus[PlayerStatus["ActivePlayerOutcome"] = 14] = "ActivePlayerOutcome";
})(PlayerStatus = exports.PlayerStatus || (exports.PlayerStatus = {}));
//These should be the same as the required ActionName in PlayerStatusController
//Also these should be the same as the PlayerStatus values in PlayerStatusEnum in the Entities solution
exports.PlayerStatusRoutesMap = (_a = {},
    _a[PlayerStatus.New] = "justone/newplayer",
    _a[PlayerStatus.InLobby] = "justone/sessionlobby",
    _a[PlayerStatus.RoundWaiting] = "justone/round/waiting",
    _a[PlayerStatus.PassivePlayerClue] = "justone/round/submitclue",
    _a[PlayerStatus.PassivePlayerWaitingForClues] = "justone/round/??",
    _a[PlayerStatus.PassivePlayerClueVote] = "justone/round/??",
    _a[PlayerStatus.PassivePlayerWaitingForClueVotes] = "justone/round/??",
    _a[PlayerStatus.PassivePlayerWaitingForActivePlayer] = "justone/round/??",
    _a[PlayerStatus.PassivePlayerOutcome] = "justone/round/??",
    _a[PlayerStatus.PassivePlayerOutcomeVote] = "justone/round/??",
    _a[PlayerStatus.ActivePlayerWaitingForClues] = "justone/round/playerwaiting",
    _a[PlayerStatus.ActivePlayerWaitingForVotes] = "justone/round/??",
    _a[PlayerStatus.ActivePlayerGuess] = "justone/round/??",
    _a[PlayerStatus.ActivePlayerWaitingForOutcomeVotes] = "justone/round/??",
    _a[PlayerStatus.ActivePlayerOutcome] = "justone/round/??",
    _a);
var PlayerStatusValidationResponse = /** @class */ (function () {
    function PlayerStatusValidationResponse() {
    }
    return PlayerStatusValidationResponse;
}());
exports.PlayerStatusValidationResponse = PlayerStatusValidationResponse;
//# sourceMappingURL=playerstatus.js.map