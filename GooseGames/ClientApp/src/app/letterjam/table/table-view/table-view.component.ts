import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { TablePlayer, TableNonPlayerCharacter, ITable, ITablePlayerBase, ITableNonPlayerCharacterBase, ICardsRequest, ITokenUpdate } from '../../../../models/letterjam/table';
import { LetterJamTableService } from '../../../../services/letterjam/table';
import { IPlayerSessionGame } from '../../../../models/session';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import _ from 'lodash';
import { AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { IClueLetter } from '../../../../models/letterjam/clues';
import { IBonusLetterGuess, ILetterCard } from '../../../../models/letterjam/letters';



export interface ITableViewParameters extends ITableComponentParameters {
}

@Component({
  selector: 'letterjam-table-view',
  templateUrl: './table-view.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './table-view.component.scss'
  ]
})
export class LetterJamTableViewComponent extends TableComponentBase implements OnInit, OnDestroy {
  @Input() parameters: ITableViewParameters;

  TableData: ITable;
  Players: TablePlayer[] = [];
  NonPlayerCharacters: TableNonPlayerCharacter[] = [];

  constructor(private tableService: LetterJamTableService) {
    super();
  }

  ngOnInit(): void {
    this.load();

    this.parameters.hubConnection.on("tokenUpdate", this.processTokenUpdate);
    this.parameters.hubConnection.on("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.on("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.on("bonusLetterGuessed", this.onBonusLetterGuessed);
  }
  ngOnDestroy(): void {
    this.parameters.hubConnection.off("tokenUpdate", this.processTokenUpdate);
    this.parameters.hubConnection.off("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.off("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.off("bonusLetterGuessed", this.onBonusLetterGuessed);
  }

  processTokenUpdate = (tokenUpdate: ITokenUpdate) => {

    if (tokenUpdate.unlockTokensFromSupply) {
      this.TableData.lockedCluesRemaining -= tokenUpdate.unlockTokensFromSupply;
      this.TableData.greenCluesRemaining += tokenUpdate.unlockTokensFromSupply;
    }

    if (tokenUpdate.addGreenTokenToPlayerId) {
      this.TableData.greenCluesRemaining -= 1;
      var player = _.find(this.Players, p => p.playerId == tokenUpdate.addGreenTokenToPlayerId);
      player.numberOfGreenCluesGiven += 1;
    }
    if (tokenUpdate.addRedTokenToPlayerId) {
      this.TableData.redCluesRemaining -= 1;
      var player = _.find(this.Players, p => p.playerId == tokenUpdate.addRedTokenToPlayerId);
      player.numberOfRedCluesGiven += 1;
    }
    if (tokenUpdate.unlockTokensFromNonPlayerCharacterIds && tokenUpdate.unlockTokensFromNonPlayerCharacterIds.length) {

      _.each(tokenUpdate.unlockTokensFromNonPlayerCharacterIds, nonPlayerCharachterId => {
        this.TableData.lockedCluesRemaining -= 1;
        this.TableData.greenCluesRemaining += 1;
        var nPC = _.find(this.NonPlayerCharacters, p => p.playerId == nonPlayerCharachterId);
        nPC.clueReleased = true;
      });
    }
  }

  onPlayerMovedOnToNextCard = (playerId: string, nextCard: ILetterCard) => {

    if (playerId !== this.parameters.request.PlayerId) {
      var player = _.find(this.Players, p => p.playerId === playerId);
      if (player) {        
        player.currentLetterIndex = player.currentLetterIndex + 1;
        player.currentLetterId = nextCard.cardId;
        player.currentLetter = nextCard;
      }
    }
  }
  onNewBonusCard = (newBonusCard: ILetterCard) => {
    if (newBonusCard.playerId !== this.parameters.request.PlayerId) {
      var player = _.find(this.Players, p => p.playerId === newBonusCard.playerId);
      if (player) {
        player.currentLetterIndex = null;
        player.currentLetterId = newBonusCard.cardId;
        player.currentLetter = newBonusCard;
        if (player.cards.length == player.numberOfLetters) {
          player.cards.push(player.cards.length);
        }
      }
    }
  }
  onBonusLetterGuessed = (bonusLetterGuess: IBonusLetterGuess) => {
    if (bonusLetterGuess.playerId !== this.parameters.request.PlayerId) {
      var player = _.find(this.Players, p => p.playerId === bonusLetterGuess.playerId);
      if (player) {
        player.currentLetterIndex = null;
        player.currentLetterId = null;
        player.currentLetter = null;
        if (player.cards.length >= player.numberOfLetters) {
          player.cards.pop();
        }
      }
    }
  }

  shownCard = (player: TablePlayer, index: number) => {
    return index === player.currentLetterIndex || index >= player.numberOfLetters;
  }

  hiddenCard = (player: TablePlayer, index: number) => {
    return !this.shownCard(player, index);
  }

  load = () => {
    this.Loading = true;
    this.tableService.GetTable(this.parameters.request)
      .then(response => this.HandleGenericResponse(response, r => {

        this.TableData = r;
        this.parameters.setCurrentRoundId(r.currentRoundId);

        var loadingProps = {
          loadingPlayer: true,
          loadingCard: true
        }

        _.each(r.players, p => {
          if (p.playerId !== this.parameters.request.PlayerId) {
            var player = <TablePlayer>{
              ...(<ITablePlayerBase>p),
              ...loadingProps,
              playerId: p.playerId,
              cards: new Array(p.currentLetterIndex !== null ? p.numberOfLetters : p.numberOfLetters + 1).fill(0).map((a, index) => index)
            };
            this.Players.push(player);
          }
        });
        _.each(r.nonPlayerCharacters, p => {

          var player = <TableNonPlayerCharacter>{
            ...(<ITableNonPlayerCharacterBase>p),
            ...loadingProps,
            playerId: p.nonPlayerCharacterId,
            cards: new Array(p.numberOfLettersRemaining ?? 1).fill(0).map((a, index) => index)
          };

          this.NonPlayerCharacters.push(player);
        });

        this.Loading = false;

        return Promise.resolve(response);
      }))
      .then(response => this.HandleGenericResponseBase(response, () => {
        return Promise.all([this.loadPlayers(), this.loadCards()]).then(() => response);
      }));
  }

  loadPlayers = (): Promise<any> => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true)).then(r => {

      _.each(this.Players, p => {
        p.player = _.find(r, fP => fP.id == p.playerId);
        p.loadingPlayer = false;
      });
      _.each(this.NonPlayerCharacters, p => {
        p.player = _.find(r, fP => fP.id == p.playerId);
        p.loadingPlayer = false;
      });
    });
  }

  loadCards = (): Promise<any> => {
    var currentLetters = _.map(this.Players, p => p.currentLetterId).concat(_.map(this.NonPlayerCharacters, p => p.currentLetterId));

    return this.parameters.getCardsFromCache({ cardIds: currentLetters, relevantCards: null }).then(r => {
      _.each(this.Players, p => {
        p.currentLetter = _.find(r, fP => fP.cardId == p.currentLetterId);
        p.loadingCard = false;
      });
      _.each(this.NonPlayerCharacters, p => {
        p.currentLetter = _.find(r, fP => fP.cardId == p.currentLetterId);
        p.loadingCard = false;
      });
    });
  }
}
