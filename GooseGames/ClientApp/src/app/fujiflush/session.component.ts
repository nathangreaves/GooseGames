import { Router, ActivatedRoute } from "@angular/router";
import { Component } from "@angular/core";
import {
  trigger,
  state,
  style,
  animate,
  transition,
  sequence,
  group,
  query,
  animateChild
  // ...
} from '@angular/animations';
import { FujiPlayer } from "../../models/fujiflush/player";
import { FujiConcealedHand, FujiHand, FujiPlayerHand } from "../../models/fujiflush/hand";
import { FujiPlayedCard, FujiCard, FujiHandCard, ActivePlayerPlayedCardAnimation } from "../../models/fujiflush/card";
import * as _ from "lodash";
import { CardNumberCss } from '../../services/fujiflush/ui'
import { IPlayerSession } from "../../models/session";
import { FujiSessionService } from "../../services/fujiflush/session";
import { FujiHandService } from "../../services/fujiflush/hand";
import { GenericResponseBase } from "../../models/genericresponse";
import * as signalR from "@microsoft/signalr";
import { FujiCardService } from "../../services/fujiflush/card";
import { FujiUpdate } from "../../models/fujiflush/fujiupdate";

@Component({
  selector: 'app-fujiflush-session-component',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css'],
  animations: [
    trigger('activePlayerPlayedCardTrigger', [
      state("*", style({ opacity: 0 })),
      transition(':enter', [
        sequence([
          style({ opacity: 0 }),
          animate("0.5s", style({ opacity: 1 })),
          animate("1s", style({ opacity: 1 })),
          animate("0.5s", style({ opacity: 0 })),
          style({ opacity: 0 })          
        ])
      ])
    ]),    
    //trigger('playedCardAreaLeave', [
    //  state("*", style({ opacity: "*" })),
    //  transition(':leave', [        
    //    group([
    //      query("@*", [animateChild()], { optional: true }),
    //      animate("8s", style({ opacity: 0 }))         
    //    ])
    //  ])
    //]),
    trigger('playedCardLeave', [
      state("*", style({ opacity: "*" })),
      state("pushed", style({ opacity: 0 })),
      state("flushed", style({ opacity: 0 })),
      state("normal", style({ opacity: "*" })),
      transition('normal => flushed', [
        style({ opacity: 0 }),
        sequence([
          style({ opacity: "*" }),
          animate("2s", style({ transform: 'rotate(-720deg) scale(0)', opacity: 0 })),  
          style({ opacity: 0 })          
        ])
      ]),
      transition('normal => pushed', [
        style({ opacity: 0 }),
        sequence([
          style({ opacity: "*" }),
          animate("100ms", style({ transform: 'scale(0.7)', opacity: "*" })),
          animate("400ms", style({ transform: 'scale(1.4)', opacity: 0 })),
          style({ opacity: 0 })          
        ])
      ])
    ]),
    trigger('combinedValueEnter', [
      state("*", style({ opacity: "*" })),
      transition(':enter', [
        sequence([
          style({ opacity: 0 }),
          animate("1s", style({ opacity: 1 }))
        ])
      ]),
      transition(':leave', [
        sequence([
          style({ opacity: "*" }),
          animate("0.2s", style({ opacity: 0 }))
        ])
      ])
    ]),
    trigger('playedCardIncrement', [
      transition(':increment', [
        animate('100ms', style({
          background: '*',
          transform: 'scale(1.3, 1.2)',
        })),
        animate('100ms', style({
          background: '*',
          transform: 'scale(1.2, 1.2)',
        })),
        animate('300ms'),
      ])
    ]),
    trigger('handCardEnter', [
      transition(':enter', [
        sequence([
          style({ opacity: 0 }),
          animate("0.3s", style({ opacity: 1 }))
        ])
      ])
    ])
  ]
})

//transition(":leave", [
//  style({ transform: "scaleY(1)", height: "*" }),
//  query("@*", [animateChild()], { optional: true }),
//  animate(transformTiming, style({ transform: "scaleY(0)", height: "0" }))
//])

export class FujiSessionComponent implements IPlayerSession {

  _sessionService: FujiSessionService;
  _handService: FujiHandService;
  _cardService: FujiCardService;

  _hubConnection: signalR.HubConnection;

  PlayerId: string;
  SessionId: string;
  Loading: boolean = true;
  ErrorMessage: string;
  Animating: boolean = false;

  Players: FujiPlayer[];
  WinningPlayers: FujiPlayer[];

  Player: FujiPlayer;

  //Used only for animation purposes. Consider alternative way?
  ActivePlayerPlayedCardAnimation: ActivePlayerPlayedCardAnimation;

  CardNumberCss = CardNumberCss;

  constructor(sessionService: FujiSessionService, handService: FujiHandService, cardService: FujiCardService, activatedRoute: ActivatedRoute) {

    this._sessionService = sessionService;
    this._handService = handService;
    this._cardService = cardService;

    this.SessionId = activatedRoute.snapshot.params.SessionId.toLowerCase();
    this.PlayerId = activatedRoute.snapshot.params.PlayerId.toLowerCase();

    this.load();
  }


  CardSelected(card: FujiHandCard) {

    if (!this.Player.isActivePlayer || this.Animating) {
      return;
    }

    if (!card.selected) {

      var hand = this.Player.hand as FujiPlayerHand;
      var selected = _.find(hand.cards, c => c.selected);
      if (selected) {
        selected.selected = false;
      }

      card.selected = true;
    }
    else {
      this.PlayCard(card);
    }
  }

  PlayCard(card: FujiHandCard) {

    //TODO: Disable playing of card.

    var hand = this.Player.hand as FujiPlayerHand;
    _.remove(hand.cards, c => c.id == card.id);
    hand.numberOfCards -= 1;

    this._cardService.PlayCard(this, card.id);
  }

  loadSession(): Promise<GenericResponseBase> {
    this.Loading = true;
    return this._sessionService.GetSession(this)
      .then(response => {
        if (response.success) {
          var loadedPlayers = response.data.players
          this.setupConcealedHands(loadedPlayers);

          var winningPlayers = _.filter(loadedPlayers, p => p.hand.numberOfCards == 0 && !p.playedCard);
          if (winningPlayers && winningPlayers.length > 0) {
            this.WinningPlayers = winningPlayers;
          }

          this.Players = loadedPlayers;

          return this.loadPlayer(loadedPlayers);
        }
        else {
          this.ErrorMessage = response.errorCode;
        }

        return response;
      })
  }

  load() {

    this.Loading = true;

    //TODO: Subscribe to hub events
    // updateSession
    // playerVictory

    this.setupConnection()
      .then(() => {
        return this.loadSession();
      })
      .then(data => {
        if (data.success) {
          this.Loading = false;
        }
      })
      .catch(err => {
        this.handleGenericError(err);
      })
      .finally(() => {

      });
  }

  private setupConnection(): Promise<any> {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/fujihub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {
      //this.Validate();
    });
    this._hubConnection.onclose(() => {
      this.handleGenericError("connection closed");
    });

    //TODO: Do nice things with the fujiUpdate response
    this._hubConnection.on("updateSession", (fujiUpdate: FujiUpdate) => {

      this.updateSession(fujiUpdate);

      //this.loadSession()
      //  .then(data => {
      //    if (data.success) {
      //      this.Loading = false;
      //    }
      //  })
      //  .catch(err => {
      //    this.handleGenericError(err);
      //  })
    });
    return this._hubConnection.start().catch(err => console.error(err));
  }

  updateSession(fujiUpdate: FujiUpdate) {

    try {
      var currentActivePlayer = _.find(this.Players, p => p.isActivePlayer === true);

      this.Animating = true;

      if (fujiUpdate.playedCards) {
        var activePlayerCard = _.find(fujiUpdate.playedCards, p => p.playerId == currentActivePlayer.id);

        if (currentActivePlayer.id != this.PlayerId) {
          var currentActivePlayerHand = currentActivePlayer.hand as FujiConcealedHand;
          currentActivePlayerHand.numberOfCards -= 1;
          currentActivePlayerHand.cards.pop();
        }

        this.ActivePlayerPlayedCardAnimation = <ActivePlayerPlayedCardAnimation>{
          faceValue: activePlayerCard.faceValue,
          playerName: currentActivePlayer.name
        };
        currentActivePlayer.playedCard = <FujiPlayedCard>{
          faceValue: activePlayerCard.faceValue,
          combinedValue: activePlayerCard.faceValue
        };

        return new Promise(function (resolve) {
          setTimeout(resolve, 2000)
        })
          .then(() => {

            var count = 0;
            _.each(fujiUpdate.playedCards, playedCard => {
              count++;
              var matchingPlayer = _.find(this.Players, player => player.id == playedCard.playerId);

              //TODO: Animate :increment on playedCard.combinedValue
              matchingPlayer.playedCard.combinedValue = playedCard.combinedValue;
            });

            //TODO: set to same as :increment animation duration
            var duration = 0;

            //return new Promise(function (resolve) {
            //  setTimeout(resolve, duration * count);
            //});

            return Promise.resolve(null);
          })
          .then(() => {
            var count = 0;
            _.each(fujiUpdate.discardedCards, discardedCard => {
              count++;
              var matchingPlayer = _.find(this.Players, player => player.id == discardedCard.playerId);

              //TODO: Animate :leave on playedCard              
              matchingPlayer.playedCard.flushed = true;
            });

            return Promise.resolve(null);
          })
          .then(() => {

            var myNewCard = null;
            var count = 0;
            _.each(fujiUpdate.newDraws, newCard => {
              count++;
              var matchingPlayer = _.find(this.Players, player => player.id == newCard.playerId);
              matchingPlayer.hand.numberOfCards += 1;

              if (matchingPlayer.id == this.PlayerId) {
                myNewCard = newCard.newCardId;
              }
              else {
                var concealedHand = matchingPlayer.hand as FujiConcealedHand;
                concealedHand.cards.push(<FujiCard>{});
              }

              //TODO: Animate :enter on hand card
            });

            if (myNewCard) {
              return this.LoadNewCard(myNewCard);
            }

            return Promise.resolve();
          })
          .then(() => {
            if (fujiUpdate.discardedCards.length > 0 || fujiUpdate.playedCards.length > 1) {

              return new Promise(function (resolve) {
                setTimeout(resolve, 2000)
              });
            }
            return Promise.resolve();
          })
          .then(() => {
            var activePlayer = _.find(this.Players, p => p.isActivePlayer);
            var newActivePlayer = _.find(this.Players, p => p.id == fujiUpdate.activePlayerUpdate.activePlayerId);

            activePlayer.isActivePlayer = false;
            newActivePlayer.isActivePlayer = true;

            this.ActivePlayerPlayedCardAnimation = null;

            if (fujiUpdate.activePlayerUpdate.discardedCards) {
              return new Promise(function (resolve) {
                setTimeout(resolve, 1000)
              });
            }
            return Promise.resolve();
          })
          .then(() => {
            var count = 0;
            _.each(fujiUpdate.activePlayerUpdate.discardedCards, discardedCard => {
              count++;
              var matchingPlayer = _.find(this.Players, player => player.id == discardedCard.playerId);

              matchingPlayer.playedCard.pushed = true;
            });

            if (fujiUpdate.activePlayerUpdate.discardedCards && fujiUpdate.gameVictoryUpdate && fujiUpdate.gameVictoryUpdate.winningPlayers && fujiUpdate.gameVictoryUpdate.winningPlayers.length) {
              return new Promise(function (resolve) {
                setTimeout(resolve, 2500)
              });
            }
            return Promise.resolve();
          })
          .then(() => {
            if (fujiUpdate.gameVictoryUpdate && fujiUpdate.gameVictoryUpdate.winningPlayers && fujiUpdate.gameVictoryUpdate.winningPlayers.length) {

              var winningPlayers = _.filter(this.Players, p => !!_.find(fujiUpdate.gameVictoryUpdate.winningPlayers, winningPlayer => p.id == winningPlayer));
              this.WinningPlayers = winningPlayers;
            }
          })
          .catch(e => {
            this.handleGenericError(e);
          })
          .finally(() => {
            this.Animating = false;
          });
      }
    } catch (e) {
      this.handleGenericError(e);
      this.Animating = false;
    }

  }

  LoadNewCard(newCardId: string): Promise<any> {    

    return this._cardService.GetCard(newCardId).then(response =>
    {
      if (response.success) {
        var hand = this.Player.hand as FujiPlayerHand;
        hand.cards.push(response.data);
      }
      else {
        this.ErrorMessage = response.errorCode;
      }
    });
  }

  CloseConnection() {
    var connection = this._hubConnection;
    if (connection) {
      connection.off("updateSession");

      connection.onclose(() => { });

      connection.stop().then(() => {
        this._hubConnection = null;
      });
    }
  }

  private loadPlayer(loadedPlayers: FujiPlayer[]): Promise<GenericResponseBase> {

    return this._handService.GetHand(this).then(response => {
      var player = _.find(loadedPlayers, p => p.id.toLowerCase() == this.PlayerId.toLowerCase());

      if (response.success) {
        player.hand = response.data;
        this.Player = player;
      }
      else {
        this.ErrorMessage = response.errorCode;
      }

      return response;
    });
  }

  private setupConcealedHands(loadedPlayers: FujiPlayer[]) {

    _.each(loadedPlayers, p => {
      var hand = p.hand as FujiConcealedHand;

      if (hand) {
        hand.cards = Array(hand.numberOfCards).fill(<FujiCard>{});
      }
    });

  }

  private handleGenericError(err: any) {
    console.error(err);
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
