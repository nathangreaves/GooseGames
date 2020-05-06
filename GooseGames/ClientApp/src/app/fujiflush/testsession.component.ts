import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router } from '@angular/router';
import { FujiSessionService } from '../../services/fujiflush/session';
import { SessionLandingResponse } from '../../models/session';
import { FujiPlayedCard } from '../../models/fujiflush/card';
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
import { CardNumberCss } from '../../services/fujiflush/ui';

@Component({
  selector: 'app-fuji-test-session-component',
  templateUrl: './testsession.component.html',
  styleUrls: ['./session.component.css'],
  animations: [
    trigger('combinedValueEnter', [
      state("*", style({ opacity: "*" })),
      transition(':enter', [
        sequence([
          style({ opacity: 0 }),
          animate("0.5s", style({ opacity: 1 }))
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
    ])    
  ]
})


      //transition(":increment", group([
      //  query(':enter', [
      //    style({
      //      left: '100%'
      //    }),
      //    animate('0.5s ease-out', style('*'))
      //  ]),
      //  query(':leave', [
      //    animate('0.5s ease-out', style({
      //      left: '-100%'
      //    }))
      //  ])
      //])),
      //transition(":decrement", group([
      //  query(':enter', [
      //    style({
      //      left: '-100%'
      //    }),
      //    animate('0.5s ease-out', style('*'))
      //  ]),
      //  query(':leave', [
      //    animate('0.5s ease-out', style({
      //      left: '100%'
      //    }))
      //  ])
      //])),

export class FujiTestSessionComponent {

  _router: Router;

  ErrorMessage: string;
  Responses: SessionLandingResponse[];
    _sessionService: FujiSessionService;

  CardNumberCss = CardNumberCss;

  constructor(sessionService: FujiSessionService, router: Router) {
    this._router = router;
    this._sessionService = sessionService;
  }

  playedCard: FujiPlayedCard;

  Go() {

    this._sessionService.CreateTestSession().then(response => {
      if (response.success) {
        this.Responses = response.data;
      }
      else {
        this.ErrorMessage = response.errorCode;
      }
    });

  }

  Add() {
    if (this.playedCard) {
      this.playedCard.combinedValue += 2;
    }
    else {
      this.playedCard = <FujiPlayedCard>{
        faceValue: 2,
        combinedValue: 2
      }
    }
  }

  Subtract() {
    if (this.playedCard) {
      this.playedCard.combinedValue -= 2;
      if (this.playedCard.combinedValue == 0) {
        this.playedCard = null;
      }
    }    
  }


}
