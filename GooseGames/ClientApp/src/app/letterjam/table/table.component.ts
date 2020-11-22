import { Component, OnInit } from '@angular/core';
import { LetterJamComponentBase } from '../../../models/letterjam/content';
import { ITableViewParameters } from './table-view/table-view.component';
import { ILetterCard } from '../../../models/letterjam/letters';
import { ICardsRequest } from '../../../models/letterjam/table';
import { LetterJamLetterCardService } from '../../../services/letterjam/letterCard';
import _ from 'lodash';
import { IProposedCluesComponentParameters } from './proposed-clues/proposed-clues.component';
import { IProposeClueComponentParameters } from './propose-clue/propose-clue.component';
import { ITableComponentParameters } from './table-base.component';

export enum TableComponentTabs {
  Table = 0,
  ProposedClues = 1,
  ProposeClue = 2
}

const LocalStorageTabKey = "goose-games-letter-jam-table-tab";

@Component({
  selector: 'letterjam-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss']
})
export class LetterJamTableComponent extends LetterJamComponentBase implements OnInit {

  tableViewParameters: ITableViewParameters;
  proposedCluesParameters: IProposedCluesComponentParameters;
  proposeClueParameters: IProposeClueComponentParameters;

  letterCards: ILetterCard[] = [];
  CurrentRoundId: string;

  CurrentTabId: number = null;
  TableLoaded: boolean;
  ProposedCluesLoaded: boolean;
  ProposeClueLoaded: boolean;

  constructor(private letterCardService: LetterJamLetterCardService) {
    super();
  }

  Table = () => {
    this.TableLoaded = true;
    this.CurrentTabId = TableComponentTabs.Table;
    this.setTabIdInLocalStorage();
  }

  ProposedClues = () => {
    this.ProposedCluesLoaded = true;
    this.CurrentTabId = TableComponentTabs.ProposedClues;
    this.setTabIdInLocalStorage();
  }

  ProposeClue = () => {
    this.ProposeClueLoaded = true;
    this.CurrentTabId = TableComponentTabs.ProposeClue;
    this.setTabIdInLocalStorage();
  }

    private setTabIdInLocalStorage() {
        localStorage.setItem(LocalStorageTabKey, this.CurrentTabId.toString());
    }

  ngOnInit(): void {

    var baseTableParameters = <ITableComponentParameters>{
      request: this,
      getCardsFromCache: this.getCardsFromCache,
      getPlayersFromCache: this.GetPlayersFromCache,
      setCurrentRoundId: this.setCurrentRoundId,
      getCurrentRoundId: this.getCurrentRoundId,
      hubConnection: this.HubConnection
    }

    this.tableViewParameters = <ITableViewParameters>{
      ...baseTableParameters
    };
    this.proposedCluesParameters = <IProposedCluesComponentParameters>{
      ...baseTableParameters,
      proposeClue: this.ProposeClue
    };
    this.proposeClueParameters = <IProposeClueComponentParameters>{
      ...baseTableParameters,
      proposedClues: this.ProposedClues
    }

    var tabItem = localStorage.getItem(LocalStorageTabKey);
    if (tabItem !== null && tabItem !== undefined) {
      this.CurrentTabId = parseInt(tabItem);
    }

    switch (this.CurrentTabId) {
      case TableComponentTabs.ProposeClue:
        this.ProposeClueLoaded = true;
        break;
      case TableComponentTabs.ProposedClues:
        this.ProposedCluesLoaded = true;
        break;
      default:
        this.CurrentTabId = TableComponentTabs.Table;
        this.TableLoaded = true;
        break;
    }

    this.getRelevantLetters();
  }

  getCurrentRoundId = (): string => {
    return this.CurrentRoundId;
  }
  setCurrentRoundId = (currentRoundId: string): void => {
    this.CurrentRoundId = currentRoundId;
  }

  getRelevantLetters(): Promise<ILetterCard[]> {
    return this.letterCardService.GetRelevantLetters(this)
      .then(response => {
        if (response.success) {
          _.each(response.data, this.addLetterCardToCache);

          return response.data;
        }
        this.SetErrorMessage(response.errorCode);
        return [];
      })
      .catch(err => {
        this.HandleGenericError(err);
        return [];
      });
  }

  getCardsFromCache = (request: ICardsRequest): Promise<ILetterCard[]> => {

    if (request.relevantCards) {
      return this.getRelevantLetters();
    }

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
