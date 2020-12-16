import { Component, OnInit, OnDestroy } from '@angular/core';
import { LetterJamComponentBase } from '../../../models/letterjam/content';

@Component({
  selector: 'letterjam-game-end',
  templateUrl: './game-end.component.html',
  styleUrls: ['../common/letterjam.common.scss',
    './game-end.component.scss']
})
export class LetterJamGameEndComponent extends LetterJamComponentBase implements OnInit, OnDestroy {

  constructor() {
    super();
  }

  ngOnInit(): void {
    Loading = false;
  }
  ngOnDestroy(): void {
  }

}
