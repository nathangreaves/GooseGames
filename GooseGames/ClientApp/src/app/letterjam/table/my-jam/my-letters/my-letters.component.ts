import { Component, OnInit, Input, ElementRef, ViewChildren, QueryList } from '@angular/core';
import { IMyJamLetterCard } from '../../../../../models/letterjam/myJam';
import { IMyJamComponentParameters } from '../my-jam.component';
import _ from 'lodash';

export interface IMyLettersComponentParameters {
  myLetters: IMyJamLetterCard[];
  currentLetterIndex: number;
  moveOnToNextLetter: () => void;
}

@Component({
  selector: 'letterjam-my-letters',
  templateUrl: './my-letters.component.html',
  styleUrls: ['../../../common/letterjam.common.scss',
    './my-letters.component.scss']
})
export class LetterJamMyLettersComponent implements OnInit {
  @ViewChildren('letterInput') letterInputs: QueryList<ElementRef>;
  @Input() parameters: IMyLettersComponentParameters;

  ConfirmingMove: boolean;
  MoveConfirmed: boolean;
  CanMoveOn: boolean;

  constructor() { }

  ngOnInit(): void {
    this.CanMoveOn = this.parameters.currentLetterIndex + 1 <= this.parameters.myLetters.length;
  }

  ngAfterViewInit(): void{
    var matchingItem = _.find(this.parameters.myLetters, (c, index) => index === this.parameters.currentLetterIndex);

    this.letterInputs.forEach(div => {
      if (div.nativeElement.id == matchingItem.cardId) {
        console.log(div.nativeElement.id);
        div.nativeElement.focus();
      }
    });
  }

  ConfirmMove = () => {
    this.parameters.moveOnToNextLetter();
    this.MoveConfirmed = true;
    this.ConfirmingMove = false;
    this.parameters.currentLetterIndex += 1;
  }

  IndexOf(index: number, indexOf: number) {
    return index === indexOf;
  }

  IndexGreaterThanCurrentLetterIndex(index: number) {
    return index > this.parameters.currentLetterIndex;
  }
}
