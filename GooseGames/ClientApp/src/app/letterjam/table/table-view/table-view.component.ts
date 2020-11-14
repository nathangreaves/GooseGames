import { Component, OnInit, Input } from '@angular/core';
import { TablePlayer, TableNonPlayerCharacter, ITable, ITablePlayerBase, ITableNonPlayerCharacterBase, ICardsRequest } from '../../../../models/letterjam/table';
import { LetterJamTableService } from '../../../../services/letterjam/table';
import { IPlayerSessionGame } from '../../../../models/session';
import { TableComponentBase } from '../table-base.component';
import _ from 'lodash';
import { IGetPlayersFromCacheRequest, AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { IGooseGamesPlayer } from '../../../../models/player';
import { ILetterCard } from '../../../../models/letterjam/letters';
import { request } from 'http';

export interface ITableViewParameters {
  request: IPlayerSessionGame
  getPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  getCardsFromCache: (request: ICardsRequest) => Promise<ILetterCard[]>;
}

@Component({
  selector: 'letterjam-table-view',
  templateUrl: './table-view.component.html',
  styleUrls: ['./table-view.component.scss']
})
export class TableViewComponent extends TableComponentBase implements OnInit {
  @Input() parameters: ITableViewParameters;

  TableData: ITable;
  Players: TablePlayer[] = [];
  NonPlayerCharacters: TableNonPlayerCharacter[] = [];

  constructor(private tableService: LetterJamTableService) {
    super();
  }

  ngOnInit(): void {
    this.load();

    //TODO: Subscribe to some stuff
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

        var loadingProps = {
          loadingPlayer: true,
          loadingCard: true
        }

        _.each(r.players, p => {
          if (p.playerId !== this.parameters.request.PlayerId) {
            var player = <TablePlayer>{
              ...(<ITablePlayerBase>p),
              ...loadingProps,
              id: p.playerId,
              cards: new Array(p.currentLetterIndex !== null ? p.numberOfLetters : p.numberOfLetters + 1).fill(0).map((a, index) => index)
            };
            this.Players.push(player);
          }
        });
        _.each(r.nonPlayerCharacters, p => {

          var player = <TableNonPlayerCharacter>{
            ...(<ITableNonPlayerCharacterBase>p),
            ...loadingProps,
            id: p.nonPlayerCharacterId,
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
        p.player = _.find(r, fP => fP.id == p.id);
        p.loadingPlayer = false;
      });
      _.each(this.NonPlayerCharacters, p => {
        p.player = _.find(r, fP => fP.id == p.id);
        p.loadingPlayer = false;
      });
    });
  }

  loadCards = (): Promise<any> => {
    var currentLetters = _.map(this.Players, p => p.currentLetterId).concat(_.map(this.NonPlayerCharacters, p => p.currentLetterId));

    return this.parameters.getCardsFromCache({ cardIds: currentLetters }).then(r => {
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
