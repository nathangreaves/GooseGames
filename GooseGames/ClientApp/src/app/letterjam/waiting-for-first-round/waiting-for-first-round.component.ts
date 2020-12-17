import { Component, OnInit, OnDestroy } from '@angular/core';
import { LetterJamComponentBase, AllPlayersFromCacheRequest, LetterJamPlayerStatus } from '../../../models/letterjam/content';
import { IWaitingForPlayerActionParameters } from '../../components/playerwaiting/player-waiting.component';
import { IGooseGamesPlayer, IGooseGamesPlayerAction } from '../../../models/player';
import { LetterJamPlayerStatusService } from '../../../services/letterjam/playerStatus';

@Component({
  selector: 'letterjam-waiting-for-first-round',
  templateUrl: './waiting-for-first-round.component.html',
  styleUrls: ['./waiting-for-first-round.component.scss']
})
export class LetterJamWaitingForFirstRoundComponent extends LetterJamComponentBase implements OnInit, OnDestroy {
  playerWaitingParameters: IWaitingForPlayerActionParameters;

  constructor(private playerStatusService: LetterJamPlayerStatusService) {
    super();
  }

  ngOnInit(): void {
    this.playerWaitingParameters = <IWaitingForPlayerActionParameters>{
      loadPlayers: this.loadPlayers,
      loadPlayerActions: this.loadPlayerActions,
      hubConnection: this.HubConnection,
      hubConnectionString: "playerHasChosenStartingWord"
    }

    this.HubConnection.on("beginNewRound", this.beginNewRound);
  }

  ngOnDestroy() {
    this.HubConnection.off("beginNewRound", this.beginNewRound);
  }

  beginNewRound = (roundId: string) => {
    this.Route(LetterJamPlayerStatus.ProposingClues);
  }

  loadPlayers = (): Promise<IGooseGamesPlayer[]> => {
    var request = new AllPlayersFromCacheRequest();
    request.allPlayers = true;
    return this.GetPlayersFromCache(request);
  };
  loadPlayerActions = (): Promise<IGooseGamesPlayerAction[]> => {
    return this.playerStatusService.PlayersWaitingForFirstRound(this)
      .then(response => {

        if (response.success) {
          return response.data;
        }
        this.SetErrorMessage(response.errorCode);
        return [];
      })
      .catch(err => {
        this.HandleGenericError(err);
        return [];
      });
  };
}
