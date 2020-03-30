"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@microsoft/signalr");
var JustOnePlayerWaitingComponentBase = /** @class */ (function () {
    function JustOnePlayerWaitingComponentBase(playerStatusService, router, activatedRoute, playerStatus) {
        var _this = this;
        this._router = router;
        this._playerStatusService = playerStatusService;
        this.SessionId = activatedRoute.snapshot.params.SessionId;
        this.PlayerId = activatedRoute.snapshot.params.PlayerId;
        this._hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/lobbyhub?sessionId=" + this.SessionId + "&playerId=" + this.PlayerId)
            .build();
        this.setupConnection(this._hubConnection);
        this._hubConnection.start().catch(function (err) { return console.error(err); });
        this._playerStatusService.Validate(this, playerStatus, function () {
            _this.onRedirect();
            _this.CloseConnection();
        })
            .then(function (data) {
            if (data.success) {
                return _this.load();
            }
        })
            .then(function () {
            _this.Loading = false;
        });
    }
    JustOnePlayerWaitingComponentBase.prototype.CloseConnection = function () {
        if (this._hubConnection) {
            this._hubConnection.stop();
            this._hubConnection = null;
        }
    };
    JustOnePlayerWaitingComponentBase.prototype.HandleGenericError = function () {
        this.ErrorMessage = "An Unknown Error Occurred";
    };
    return JustOnePlayerWaitingComponentBase;
}());
exports.JustOnePlayerWaitingComponentBase = JustOnePlayerWaitingComponentBase;
//# sourceMappingURL=playerwaiting.component.base.js.map