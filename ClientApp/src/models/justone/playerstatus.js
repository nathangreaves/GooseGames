"use strict";
var _a;
Object.defineProperty(exports, "__esModule", { value: true });
var PlayerStatus;
(function (PlayerStatus) {
    PlayerStatus[PlayerStatus["New"] = 0] = "New";
    PlayerStatus[PlayerStatus["InLobby"] = 1] = "InLobby";
    PlayerStatus[PlayerStatus["RoundWaiting"] = 2] = "RoundWaiting";
    PlayerStatus[PlayerStatus["RoundSubmitClue"] = 3] = "RoundSubmitClue";
    PlayerStatus[PlayerStatus["RoundPlayerWaiting"] = 4] = "RoundPlayerWaiting";
})(PlayerStatus = exports.PlayerStatus || (exports.PlayerStatus = {}));
//These should be the same as the required ActionName in PlayerStatusController
//Also these should be the same as the PlayerStatus values in PlayerStatusEnum in the Entities solution
exports.PlayerStatusRoutesMap = (_a = {},
    _a[PlayerStatus.InLobby] = "justone/sessionlobby",
    _a[PlayerStatus.New] = "justone/newplayer",
    _a[PlayerStatus.RoundWaiting] = "justone/round/waiting",
    _a[PlayerStatus.RoundSubmitClue] = "justone/round/submitclue",
    _a[PlayerStatus.RoundPlayerWaiting] = "justone/round/playerwaiting",
    _a);
var PlayerStatusValidationResponse = /** @class */ (function () {
    function PlayerStatusValidationResponse() {
    }
    return PlayerStatusValidationResponse;
}());
exports.PlayerStatusValidationResponse = PlayerStatusValidationResponse;
//# sourceMappingURL=playerstatus.js.map