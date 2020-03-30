import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { PlayerStatusRoutesMap, PlayerStatus } from '../models/justone/playerstatus';

import { JustOneLandingComponent } from './justone/landing.component';

import { JustOneNewPlayerDetailsComponent } from './justone/newplayerdetails.component';
import { JustOneSessionLobbyComponent } from './justone/sessionlobby.component';
import { JustOneRoundWaitingComponent } from './justone/round/waiting.component';

import { JustOneSubmitClueComponent } from './justone/round/submitclue.component';
import { JustOnePassivePlayerWaitingForCluesComponent } from './justone/round/passiveplayerwaitingforclues.component';
import { JustOneClueVoteComponent } from './justone/round/cluevote.component';
import { JustOnePassivePlayerWaitingForClueVoteComponent } from './justone/round/passiveplayerwaitingforcluevote.component';
import { JustOnePassivePlayerWaitingForActivePlayerComponent } from './justone/round/passiveplayerwaitingforactiveplayer.component';
import { JustOneActivePlayerRoundOutcomeComponent, JustOnePassivePlayerRoundOutcomeComponent } from './justone/round/outcome.component'
import { JustOnePassivePlayerOutcomeVoteComponent } from './justone/round/passiveplayeroutcomevote.component';
import { JustOnePassivePlayerWaitingForOutcomeVoteComponent, JustOneActivePlayerWaitingForOutcomeVoteComponent } from './justone/round/playerwaitingforoutcomevotes.component'

import { JustOneActivePlayerWaitingForCluesComponent } from './justone/round/activeplayerwaitingforclues.component';
import { JustOneActivePlayerWaitingForClueVoteComponent } from './justone/round/activeplayerwaitingforcluevote.component';
import { JustOneActivePlayerGuess } from './justone/round/activeplayerguess.component';

import { JustOnePlayerWaitingComponent } from './justone/round/playerwaiting.component';
import { JustOneClueListComponent } from './justone/round/cluelist.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    JustOneLandingComponent,

    JustOneNewPlayerDetailsComponent,
    JustOneSessionLobbyComponent,
    JustOneRoundWaitingComponent,
    JustOneSubmitClueComponent,
    JustOneActivePlayerWaitingForCluesComponent,
    JustOneActivePlayerWaitingForClueVoteComponent,
    JustOnePassivePlayerWaitingForCluesComponent,
    JustOnePassivePlayerWaitingForClueVoteComponent,
    JustOnePassivePlayerWaitingForActivePlayerComponent,
    JustOneActivePlayerGuess,
    JustOneClueVoteComponent,
    JustOneActivePlayerRoundOutcomeComponent,
    JustOnePassivePlayerRoundOutcomeComponent,
    JustOnePassivePlayerOutcomeVoteComponent,
    JustOnePassivePlayerWaitingForOutcomeVoteComponent,
    JustOneActivePlayerWaitingForOutcomeVoteComponent,

    JustOnePlayerWaitingComponent,
    JustOneClueListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'justone', component: JustOneLandingComponent },

      { path: PlayerStatusRoutesMap[PlayerStatus.New], component: JustOneNewPlayerDetailsComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.InLobby], component: JustOneSessionLobbyComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.RoundWaiting], component: JustOneRoundWaitingComponent },

      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerClue], component: JustOneSubmitClueComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerWaitingForClues], component: JustOnePassivePlayerWaitingForCluesComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerClueVote], component: JustOneClueVoteComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerWaitingForClueVotes], component: JustOnePassivePlayerWaitingForClueVoteComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerWaitingForActivePlayer], component: JustOnePassivePlayerWaitingForActivePlayerComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerOutcome], component: JustOnePassivePlayerRoundOutcomeComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerOutcomeVote], component: JustOnePassivePlayerOutcomeVoteComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerWaitingForOutcomeVotes], component: JustOnePassivePlayerWaitingForOutcomeVoteComponent },


      { path: PlayerStatusRoutesMap[PlayerStatus.ActivePlayerWaitingForClues], component: JustOneActivePlayerWaitingForCluesComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.ActivePlayerWaitingForVotes], component: JustOneActivePlayerWaitingForClueVoteComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.ActivePlayerGuess], component: JustOneActivePlayerGuess },
      { path: PlayerStatusRoutesMap[PlayerStatus.ActivePlayerWaitingForOutcomeVotes], component: JustOneActivePlayerWaitingForOutcomeVoteComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.ActivePlayerOutcome], component: JustOneActivePlayerRoundOutcomeComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
