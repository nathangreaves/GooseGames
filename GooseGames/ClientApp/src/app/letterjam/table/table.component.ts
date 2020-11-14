import { Component, OnInit } from '@angular/core';
import { LetterJamComponentBase } from '../../../models/letterjam/content';
import { ITableViewParameters } from './table-view/table-view.component';
import { ILetterCard } from '../../../models/letterjam/letters';
import { ICardsRequest } from '../../../models/letterjam/table';
import { LetterJamLetterCardService } from '../../../services/letterjam/letterCard';
import _ from 'lodash';

@Component({
  selector: 'letterjam-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss']
})
export class LetterJamTableComponent extends LetterJamComponentBase implements OnInit {

  tableViewParameters: ITableViewParameters;

  letterCards: ILetterCard[] = [];

  constructor(private letterCardService: LetterJamLetterCardService) {
    super();
  }

  ngOnInit(): void {

    this.tableViewParameters = <ITableViewParameters>{
      request: this,
      getCardsFromCache: this.getCardsFromCache,
      getPlayersFromCache: this.GetPlayersFromCache
    }

    this.getRelevantCards();
  }
  getRelevantCards() {
    this.letterCardService.GetReleventLetters(this)
      .then(response => this.HandleGenericResponse(response, r =>
      {
        _.each(r, this.addLetterCardToCache);

        return response;
      }))
      .catch(this.HandleGenericError);
  }

  getCardsFromCache = (request: ICardsRequest): Promise<ILetterCard[]> => {

    var response = [];
    var newRequest = <ICardsRequest>{
      cardIds: []
    }
    _.each(request.cardIds, requestedCardId => {

      var found = _.find(this.letterCards, lC => lC.cardId == requestedCardId);
      if (found) {
        response.push(found);
      }
      else {
        newRequest.cardIds.push(requestedCardId);
      }
    });

    if (newRequest.cardIds.length > 0) {
      return this.letterCardService.GetLetters(this, newRequest).then(res => this.HandleGenericResponse(res, r => {
        _.each(r, lC => {
          this.addLetterCardToCache(lC);
          response.push(lC);
        });
        return res;
      })).then(() => {
        return response;
      });
    }

    return Promise.resolve(response);
  };

  addLetterCardToCache = (letterCard: ILetterCard) => {
    var find = _.find(this.letterCards, lC => lC.cardId === letterCard.cardId);
    if (!find) {
      this.letterCards.push(letterCard);
    }
  }
}
