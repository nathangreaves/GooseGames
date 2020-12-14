import { Component, OnInit, Input, ElementRef, ViewChildren, QueryList } from '@angular/core';
import { IMyJamLetterCard, IFinalWordPublicLetter, IFinalWordLetter } from '../../../../../models/letterjam/myJam';
import { IMyJamComponentParameters } from '../my-jam.component';
import _ from 'lodash';
import { GetColourFromLetterIndex, StyleLetterCardWithColour } from '../../../../../services/letterjam/colour';
import { LetterJamMyJamService } from '../../../../../services/letterjam/myJam';
import { IPlayerSessionGame } from '../../../../../models/session';

export interface IMyLettersComponentParameters {
  myLetters: IMyJamLetterCard[];
  finalWordLetters: IMyJamLetterCard[]
  currentLetterIndex: number;
  moveOnToNextLetter: () => void;
  sessionInfo: IPlayerSessionGame;
  gameEnd: boolean;
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
  PublicLetters: IFinalWordPublicLetter[];

  constructor(private myJamService: LetterJamMyJamService) { }

  ngOnInit(): void {
    this.CanMoveOn = !this.parameters.gameEnd && this.parameters.currentLetterIndex != null && this.parameters.currentLetterIndex + 1 <= this.parameters.myLetters.length;
    if (this.parameters.gameEnd) {
      this.myJamService.GetFinalWordPublicLetters(this.parameters.sessionInfo)
        .then(response => {
          if (response.success) {
            this.PublicLetters = response.data;
          }
        });
    }
    else {
      this.PublicLetters = [];
    }
  }

  ngAfterViewInit(): void{
    var matchingItem = _.find(this.parameters.myLetters, (c, index) => index === this.parameters.currentLetterIndex || c.bonusLetter);

    if (matchingItem) {
      this.letterInputs.forEach(div => {
        if (div.nativeElement.id == matchingItem.cardId) {
          console.log(div.nativeElement.id);
          div.nativeElement.focus();
        }
      });
    }
  }

  LetterStyle = (letter: IMyJamLetterCard, index: number) => {
    if (letter.bonusLetter) {
      return {};
    }
    var color = GetColourFromLetterIndex(index + 1);
    return StyleLetterCardWithColour(color);
  }

  ConfirmMove = () => {
    this.parameters.moveOnToNextLetter();
    this.MoveConfirmed = true;
    this.ConfirmingMove = false;
    if (this.parameters.currentLetterIndex != null) {
      this.parameters.currentLetterIndex += 1;
    }
  }

  IndexOf(index: number, indexOf: number) {
    return index === indexOf;
  }

  IndexGreaterThanCurrentLetterIndex(index: number) {
    return this.parameters.currentLetterIndex != null && index > this.parameters.currentLetterIndex;
  }  

  IsPublicLetterInFinalWord(letter: IFinalWordPublicLetter) {
    if (letter.isWildCard) {
      return _.find(this.parameters.finalWordLetters, c => c.isWildCard);
    }
    return _.find(this.parameters.finalWordLetters, c => c.cardId === letter.cardId);
  }

  IsLetterInFinalWord(letter: IMyJamLetterCard) {
    return _.find(this.parameters.finalWordLetters, c => c.cardId === letter.cardId);
  }

  AddPublicLetterToFinalWord(letter: IFinalWordPublicLetter) {
    this.parameters.finalWordLetters.push({
      cardId: letter.cardId,
      bonusLetter: !letter.isWildCard,
      isWildCard: letter.isWildCard,
      playerLetterGuess: letter.letter
    });
  }
  AddLetterToFinalWord(letter: IMyJamLetterCard) {
    this.parameters.finalWordLetters.push(letter);
  }
  DeleteLastLetter() {
    this.parameters.finalWordLetters.pop();
  }
}
