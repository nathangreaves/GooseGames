import { Router, ActivatedRoute } from "@angular/router";
import { Component } from "@angular/core";
import { FujiPlayer } from "../../models/fujiflush/player";
import { FujiConcealedHand, FujiHand, FujiPlayerHand } from "../../models/fujiflush/hand";
import { FujiPlayedCard, FujiCard, FujiHandCard } from "../../models/fujiflush/card";
import * as _ from "lodash";
import { CardNumberCss } from '../../services/fujiflush/ui'
import { IPlayerSession } from "../../models/session";
import { FujiSessionService } from "../../services/fujiflush/session";
import { FujiHandService } from "../../services/fujiflush/hand";
import { GenericResponseBase } from "../../models/genericresponse";

@Component({
  selector: 'app-fujiflush-session-component',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css']
})

export class FujiSessionComponent implements IPlayerSession {

  _sessionService: FujiSessionService;
  _handService: FujiHandService;

  PlayerId: string;
  SessionId: string;
  Loading: boolean = true;
  ErrorMessage: string;

  Players: FujiPlayer[]

  Player: FujiPlayer;

  CardNumberCss = CardNumberCss;

  constructor(sessionService: FujiSessionService, handService: FujiHandService, activatedRoute: ActivatedRoute) {

    this._sessionService = sessionService;
    this._handService = handService;

    this.SessionId = activatedRoute.snapshot.params.SessionId.toLowerCase();
    this.PlayerId = activatedRoute.snapshot.params.PlayerId.toLowerCase();

    this.load();
  }


  CardSelected(card: FujiHandCard) {

    if (!this.Player.isActivePlayer) {
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

    //TODO: Post to server

    var hand = this.Player.hand as FujiPlayerHand;
    _.remove(hand.cards, card);

    this.Player.playedCard = <FujiPlayedCard>{ faceValue: card.faceValue, combinedValue: null };
  }
  

  load() {

    this.Loading = true;

    //TODO: Subscribe to hub events
    // updateSession
    // playerVictory

    this._sessionService.GetSession(this)
      .then(response => {
        if (response.success) {
          var loadedPlayers = response.data.players
          this.setupConcealedHands(loadedPlayers);

          this.Players = loadedPlayers;

          return this.loadPlayer(loadedPlayers);
        }
        else {
          this.ErrorMessage = response.errorCode;
        }

        return response;
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

  private discardCard() {
    //TODO: Send delete to server to discard card if still have one in play
  }

  private handleGenericError(err: any) {
    console.error(err);
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
