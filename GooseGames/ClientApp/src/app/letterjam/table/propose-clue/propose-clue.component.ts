import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterCard, ILetterCard } from '../../../../models/letterjam/letters';
import { AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import _ from 'lodash';
import { IGooseGamesPlayer } from '../../../../models/player';
import { LetterJamCluesService } from '../../../../services/letterjam/clues';

export interface IProposeClueComponentParameters extends ITableComponentParameters {
  proposedClues: () => void;
}

@Component({
  selector: 'letterjam-propose-clue',
  templateUrl: './propose-clue.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './propose-clue.component.scss']
})
export class LetterJamProposeClueComponent extends TableComponentBase implements OnInit, OnDestroy {
  @Input() parameters: IProposeClueComponentParameters;

  BuiltWord: LetterCard[] = [];
  RelevantLetters: LetterCard[] = [];
  HumanPlayers: IGooseGamesPlayer[] = [];
  ShowAsPlayerId: string = null;
  DisableButtons: boolean = false;
  WildCardId: string;

  constructor(private clueService: LetterJamCluesService) {
    super();
    this.WildCardId = clueService.WildCardId;
  }

  ngOnInit(): void {
    this.loadRelevantLetters().then(this.loadPlayers).then(this.loadHumanPlayers);
    this.parameters.hubConnection.on("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.on("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.on("newNpcCard", this.onNewNpcCard);
    this.parameters.hubConnection.on('giveClue', this.onGiveClue);
    this.parameters.hubConnection.on('beginNewRound', this.onBeginNewRound);
  }
  ngOnDestroy(): void {
    this.parameters.hubConnection.off("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.off("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.off("newNpcCard", this.onNewNpcCard);
    this.parameters.hubConnection.off('giveClue', this.onGiveClue);
    this.parameters.hubConnection.off('beginNewRound', this.onBeginNewRound);
  }
  onPlayerMovedOnToNextCard = (playerId: string, nextCard: ILetterCard) => {
    if (playerId !== this.parameters.request.PlayerId && nextCard) {
      var relevantCard = _.find(this.RelevantLetters, p => p.playerId === playerId);
      if (relevantCard) {
        this.BuiltWord = [];
        relevantCard.bonusLetter = nextCard.bonusLetter;
        relevantCard.cardId = nextCard.cardId;
        relevantCard.letter = nextCard.letter;
      }
    }
  }
  onNewBonusCard = (newBonusCard: ILetterCard) => {
    if (newBonusCard.playerId !== this.parameters.request.PlayerId) {
      var relevantCard = _.find(this.RelevantLetters, p => p.playerId === newBonusCard.playerId);
      if (relevantCard) {
        this.BuiltWord = [];
        relevantCard.bonusLetter = newBonusCard.bonusLetter;
        relevantCard.cardId = newBonusCard.cardId;
        relevantCard.letter = newBonusCard.letter;
      }
    }
  }
  onNewNpcCard = (newNpcCard: ILetterCard) => {
    var relevantCard = _.find(this.RelevantLetters, p => p.nonPlayerCharacterId === newNpcCard.nonPlayerCharacterId);
    if (relevantCard) {
      this.BuiltWord = [];
      relevantCard.cardId = newNpcCard.cardId;
      relevantCard.letter = newNpcCard.letter;
    }
  }
  onGiveClue = () => {
    this.BuiltWord = [];
  }
  onBeginNewRound = () => {
    this.BuiltWord = [];
  }

  loadRelevantLetters = (): Promise<any> => {
    return this.parameters.getCardsFromCache({ relevantCards: true, cardIds: null })
      .then(r => {
        _.each(r, letterCard => {
          this.RelevantLetters.push({
            ...letterCard,
            loadingPlayer: true,
            player: null
          });
        });

        this.RelevantLetters.push({
          bonusLetter: false,
          cardId: this.WildCardId,
          letter: "*",
          nonPlayerCharacterId: null,
          playerId: this.WildCardId,
          loadingPlayer: false,
          player: {
            id: this.WildCardId,
            name: null,
            emoji: null,
            playerNumber: 100
          }
        })
      });
  }

  loadPlayers = (): Promise<any> => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true)).then(r => {

      _.each(this.RelevantLetters, l => {
        if (!(l.bonusLetter && !l.playerId) && l.cardId !== this.WildCardId) {
          l.player = _.find(r, fP => fP.id == l.playerId || fP.id == l.nonPlayerCharacterId);
        }
        l.loadingPlayer = false;
      });

      this.RelevantLetters = _.sortBy(this.RelevantLetters, l => l.playerId ? l.player.playerNumber : l.nonPlayerCharacterId ? l.player.playerNumber + 10 : 20);
    });
  }

  loadHumanPlayers = (): Promise<any> => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, false)).then(r => {

      _.each(r, p => {
        if (p.id !== this.parameters.request.PlayerId)
          this.HumanPlayers.push(p);
      });

    });
  }

  show(playerId: string) {
    this.ShowAsPlayerId = playerId;
  }

  hide() {
    this.ShowAsPlayerId = null;
  }

  DeleteLastLetter = () => {
    if (this.BuiltWord.length > 0) {
      this.BuiltWord = this.BuiltWord.slice(0, this.BuiltWord.length - 1);
    }
  }

  AddLetterToWord = (letter: LetterCard) => {
    this.BuiltWord.push(letter);
  }

  SubmitClue = () => {
    if (this.BuiltWord.length < 1) {
      this.ErrorMessage = "Please submit a word with at least 1 letter!"
    }
    this.DisableButtons = true;

    this.clueService.SubmitClue(this.parameters.request,
      this.parameters.getCurrentRoundId(),
      this.BuiltWord)
      .then(response => this.HandleGenericResponseBase(response, () => {
        this.parameters.proposedClues();
        this.BuiltWord = [];
        return response;
      }))
      .catch(this.HandleGenericError)
      .finally(() => {
        this.DisableButtons = false;
      });
  }

  Back = () => {
    this.BuiltWord = [];
    this.parameters.proposedClues();
  }
}
