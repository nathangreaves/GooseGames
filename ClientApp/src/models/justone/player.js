"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var PlayerDetails = /** @class */ (function () {
    function PlayerDetails() {
    }
    return PlayerDetails;
}());
exports.PlayerDetails = PlayerDetails;
var UpdatePlayerDetailsRequest = /** @class */ (function (_super) {
    __extends(UpdatePlayerDetailsRequest, _super);
    function UpdatePlayerDetailsRequest() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return UpdatePlayerDetailsRequest;
}(PlayerDetails));
exports.UpdatePlayerDetailsRequest = UpdatePlayerDetailsRequest;
var UpdatePlayerDetailsResponse = /** @class */ (function () {
    function UpdatePlayerDetailsResponse() {
    }
    return UpdatePlayerDetailsResponse;
}());
exports.UpdatePlayerDetailsResponse = UpdatePlayerDetailsResponse;
var GetPlayerDetailsRequest = /** @class */ (function () {
    function GetPlayerDetailsRequest() {
    }
    return GetPlayerDetailsRequest;
}());
exports.GetPlayerDetailsRequest = GetPlayerDetailsRequest;
var PlayerDetailsResponse = /** @class */ (function () {
    function PlayerDetailsResponse() {
    }
    return PlayerDetailsResponse;
}());
exports.PlayerDetailsResponse = PlayerDetailsResponse;
var GetPlayerDetailsResponse = /** @class */ (function () {
    function GetPlayerDetailsResponse() {
    }
    return GetPlayerDetailsResponse;
}());
exports.GetPlayerDetailsResponse = GetPlayerDetailsResponse;
//# sourceMappingURL=player.js.map