import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterJamMyJamService } from '../../../../services/letterjam/myJam';
import { MyJamRound, IMyJamLetterCard } from '../../../../models/letterjam/myJam';
import _ from 'lodash';
import { ILetterJamClueComponentParameters } from '../clue/clue.component';
import { AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';

export interface IMyJamComponentParameters extends ITableComponentParameters {
  proposeClue: () => void;
}

@Component({
  selector: 'letterjam-my-jam',
  templateUrl: './my-jam.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './my-jam.component.scss']
})
export class LetterJamMyJamComponent extends TableComponentBase implements OnInit, OnDestroy {

  @Input() parameters: IMyJamComponentParameters;

  Rounds: MyJamRound[] = [];
  MyLetters: IMyJamLetterCard[];
  CurrentLetterIndex: number;

  constructor(private myJamService: LetterJamMyJamService) {
    super();
  }

  ngOnInit(): void {
    this.parameters.hubConnection.on("giveClue", this.clueGiven);
    this.load();
  }
  ngOnDestroy(): void {
    this.parameters.hubConnection.off("giveClue", this.clueGiven);
  }

  getClueComponentProperties = (round: MyJamRound) => {
    return <ILetterJamClueComponentParameters>{
      clue: {
        id: round.clueId,
        letters: []
      },
      getCardsFromCache: this.parameters.getCardsFromCache,
      getPlayersFromCache: this.parameters.getPlayersFromCache,
      request: this.parameters.request
    }
  }

  load = () => {
    this.myJamService.LoadMyJam(this.parameters.request)
      .then(response => this.HandleGenericResponse(response, r => {

        this.MyLetters = r.myLetters;
        this.CurrentLetterIndex = r.currentLetterIndex;

        _.each(r.rounds, round => {

          var roundViewModel = <MyJamRound>{
            clueGiverPlayerId: round.clueGiverPlayerId,
            clueId: round.clueId,
            letters: [],
            loadingLetters: true,
            loadingPlayer: true,
            player: null
          };

          this.Rounds.push(roundViewModel);
        });

        return response;
      }))
      .then(response => this.HandleGenericResponseBase(response, () => {
        //this.subscribe();
        return this.loadPlayers().then(() => response);
      }));
  }

  loadPlayers= () => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true)).then(r => {
      _.each(this.Rounds, round => {
        round.player = _.find(r, fP => fP.id == round.clueGiverPlayerId);
        round.loadingPlayer = false;
      });
    });
  }

  clueGiven = (clueId: string, clueGiverPlayerId: string) => {
    var roundViewModel = <MyJamRound>{
      clueGiverPlayerId: clueGiverPlayerId,
      clueId: clueId,
      letters: [],
      loadingLetters: true,
      loadingPlayer: true,
      player: null
    };

    this.Rounds.push(roundViewModel);
  }

  EditMyLetters = () => {
    //TODO: Open modal to edit letters and potentially move on to next letter?
  }
}
