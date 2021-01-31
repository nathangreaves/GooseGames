import { Component, OnInit } from '@angular/core';
import { LetterJamComponentBase, LetterJamPlayerStatus, AllPlayersFromCacheRequest } from '../../../models/letterjam/content';
import { LetterJamStartWordService } from '../../../services/letterjam/startWord';
import { GenericResponseBase } from '../../../models/genericresponse';
import { IGooseGamesPlayer } from '../../../models/player';

@Component({
  selector: 'letterjam-submit-word',
  templateUrl: './submit-word.component.html',
  styleUrls: ['./submit-word.component.scss']
})
export class LetterJamSubmitWordComponent extends LetterJamComponentBase implements OnInit {

  ButtonsEnabled: boolean = true;
  ForPlayerId: string;
  NumberOfLetters: number;
  ForPlayer: IGooseGamesPlayer;
  Word: string;
  RouteActivated: boolean;

  constructor(private startWordService: LetterJamStartWordService) {
    super();
    this.KnownErrorCodes.push({
      errorCode: "182ef180-38a9-470f-a217-17566a99ab57",
      errorMessage: "The requested letters cannot be reserved, another player may have already chosen them. Please try a different word"
    });
    this.KnownErrorCodes.push({
      errorCode: "697a4c1e-d4d8-4cac-9a80-c3927299ee20",
      errorMessage: "Please submit a word with the correct number of letters."
    });
  }

  ngOnInit() {
    this.load()
      .then(() => {
        this.Loading = false;
      });

    this.HubConnection.on("beginNewRound", this.onBeginNewRound);
  }

  ngOnDestroy(): void {
    this.HubConnection.off("beginNewRound", this.onBeginNewRound);
  }

  onBeginNewRound = () => {
    this.RouteActivated = true;    
    this.Route(LetterJamPlayerStatus.ProposingClues);
  }

  loadPlayerFromCache = (): Promise<any> => {
    var request = new AllPlayersFromCacheRequest();
    request.playerIds = [this.ForPlayerId];
    request.allPlayers = false;
    request.includeNPC = false;
    request.includeReal = true;

    return this.GetPlayersFromCache(request)
      .then(response => {
        this.ForPlayer = response[0];
      })
      .catch(this.HandleGenericError);
  }

  load(): Promise<any> {

    return this.startWordService.GetConfiguration(this)
      .then(response => {
        return this.HandleGenericResponse(response, r => {

          this.ForPlayerId = response.data.forPlayerId;
          this.NumberOfLetters = response.data.numberOfLetters;

          return this.loadPlayerFromCache()
            .then(this.GetRandomWord)
            .then(() => response);
        });
      });
  }

  GetRandomWord = () => {
    return this.startWordService.GetRandomWord(this, this.NumberOfLetters)
      .then(response => this.HandleGenericResponse(response, r => {

        this.Word = r.randomWord;

        return Promise.resolve(response);
      }));
  }

  SubmitWord = () => {
    this.ButtonsEnabled = false;

    this.startWordService.PostStartWord(this, this.Word, this.ForPlayerId)
      .then(response => this.HandleGenericResponseBase(response, () => {

        if (!this.RouteActivated) {          
          this.Route(LetterJamPlayerStatus.WaitingForFirstRound);
        }

        return Promise.resolve(response);
      }))
      .catch(this.HandleGenericError)
      .finally(() => {
        this.ButtonsEnabled = true;
      });
  }

  RandomWord = () => {
    this.ButtonsEnabled = false;

    this.GetRandomWord()
      .catch(this.HandleGenericError)
      .finally(() => {
        this.ButtonsEnabled = true;
      });
  }
}
