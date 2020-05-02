import { Router } from "@angular/router";
import { Component } from "@angular/core";
import { FujiPlayer } from "../../models/fujiflush/player";
import { FujiConcealedHand, FujiHand, FujiPlayerHand } from "../../models/fujiflush/hand";
import { FujiPlayedCard, FujiCard, FujiHandCard } from "../../models/fujiflush/card";
import * as _ from "lodash";
import { CardNumberCss } from '../../services/fujiflush/ui'
import { IPlayerSession } from "../../models/session";

@Component({
  selector: 'app-fujiflush-demo-component',
  templateUrl: './demo.component.html',
  styleUrls: ['./demo.component.css']
})

export class FujiDemoComponent implements IPlayerSession {

  Loading: boolean;

  Players: FujiPlayer[]

  Player: FujiPlayer;

  CardNumberCss = CardNumberCss;
  PlayerId: string;
  SessionId: string;

  constructor() {

    this.PlayerId = "321e22a7-bc4f-4910-9a15-4b76846f3697";
    this.SessionId = "d02ec11c-858c-4471-a3c5-a3d43ab2dcd4";

    this.Load();
  }

  Load() {
    var loadedPlayers = <FujiPlayer[]>[
      <FujiPlayer>{
        id: "879963f0-7281-4864-af0c-67b6cf7fe7ff",
        name: "Nathan",
        isActivePlayer: false,
        playerNumber: 1,
        hand: <FujiConcealedHand>{
          numberOfCards: 6
        },
        playedCard: <FujiPlayedCard>{
          faceValue: 12
        }
      },
      <FujiPlayer>{
        id: "0b6d08aa-acd4-4429-bd4e-32ccba8cd51a",
        name: "Lauren",
        isActivePlayer: false,
        playerNumber: 2,
        hand: <FujiConcealedHand>{
          numberOfCards: 4
        },
        playedCard: <FujiPlayedCard>{
          faceValue: 2,
          combinedValue: 4
        }
      },
      <FujiPlayer>{
        id: "89b0d308-6223-43f6-8539-e3f6eae338ec",
        name: "Greg",
        isActivePlayer: true,
        playerNumber: 3,
        hand: <FujiConcealedHand>{
          numberOfCards: 4,
        },
        playedCard: null
      },
      <FujiPlayer>{
        id: "d2acfcaa-388b-476d-beb3-58e8776a79ce",
        name: "Beccaroon",
        isActivePlayer: false,
        playerNumber: 4,
        hand: <FujiConcealedHand>{
          numberOfCards: 3
        },
        playedCard: <FujiPlayedCard>{
          faceValue: 2,
          combinedValue: 4
        }
      },
      <FujiPlayer>{
        id: this.PlayerId,
        name: "Silly name david",
        isActivePlayer: false,
        playerNumber: 5,
        hand: <FujiConcealedHand>{
          numberOfCards: 5
        },
        playedCard: null
      }
    ];

    this.setupConcealedHands(loadedPlayers);

    this.Players = loadedPlayers;

    var player = _.find(loadedPlayers, p => p.id == this.PlayerId);

    if (player == null) {
      //TODO: Handle Error
    }
    else {

      player.hand = this.LoadPlayerHand();

      this.Player = player;
    }
  }
  LoadPlayerHand(): FujiPlayerHand {
    return <FujiPlayerHand>
      {
        numberOfCards: 5,
        cards: <FujiHandCard[]>[
          <FujiHandCard>{
            faceValue: 2
          },
          <FujiHandCard>{
            faceValue: 2
          },
          <FujiHandCard>{
            faceValue: 2
          },
          <FujiHandCard>{
            faceValue: 8
          },
          <FujiHandCard>{
            faceValue: 13,
            selected: true
          },
          <FujiHandCard>{
            faceValue: 19
          }
        ]
      }
  }
  setupConcealedHands(loadedPlayers: FujiPlayer[]) {

    _.each(loadedPlayers, p => {
      var hand = p.hand as FujiConcealedHand;

      if (hand) {
        hand.cards = Array(hand.numberOfCards).fill(<FujiCard>{});
      }
    });

  }

  CardSelected(card: FujiHandCard) {

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

    var hand = this.Player.hand as FujiPlayerHand;
    _.remove(hand.cards, card);

    this.Player.playedCard = <FujiPlayedCard>{ faceValue: card.faceValue, combinedValue: null };
  }

}
