import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { JustOneLandingComponent } from './landing.component';
import { JustOneNavbarHeaderComponent } from './navbarheader.component';
import { JustOneDeclarationComponent } from './declaration.component';
import { JustOneRejoinComponent } from './rejoin.component';

import { JustOneSessionLobbyComponent } from './sessionlobby.component';
import { JustOneRoundWaitingComponent } from './round/waiting.component';
import { JustOneWaitingForRoundComponent } from './round/waitingforround.component';

import { JustOneSubmitClueComponent } from './round/submitclue.component';
import { JustOnePassivePlayerWaitingForCluesComponent } from './round/passiveplayerwaitingforclues.component';
import { JustOneClueVoteComponent } from './round/cluevote.component';
import { JustOnePassivePlayerWaitingForClueVoteComponent } from './round/passiveplayerwaitingforcluevote.component';
import { JustOnePassivePlayerWaitingForActivePlayerComponent } from './round/passiveplayerwaitingforactiveplayer.component';
import { JustOneActivePlayerRoundOutcomeComponent, JustOnePassivePlayerRoundOutcomeComponent } from './round/outcome.component'
import { JustOnePassivePlayerOutcomeVoteComponent } from './round/passiveplayeroutcomevote.component';
import { JustOnePassivePlayerWaitingForOutcomeVoteComponent, JustOneActivePlayerWaitingForOutcomeVoteComponent } from './round/playerwaitingforoutcomevotes.component'

import { JustOneActivePlayerWaitingForCluesComponent } from './round/activeplayerwaitingforclues.component';
import { JustOneActivePlayerWaitingForClueVoteComponent } from './round/activeplayerwaitingforcluevote.component';
import { JustOneActivePlayerGuess } from './round/activeplayerguess.component';

import { JustOnePlayerWaitingComponent } from './round/playerwaiting.component';
import { JustOneClueListComponent } from './round/cluelist.component';

import { GlobalLobbyHubComponent } from '../components/lobby/hub';
import { LobbyComponent } from '../components/lobby/lobby';import { TristateSwitch } from '../../assets/tristate-switch.component';
import { NavbarsModule } from '../navbars.module';
import { SharedModule } from '../shared.module';

const routes: Routes = [
  { path: '', component: JustOneLandingComponent },
  
  { path: "disclaimer", component: JustOneDeclarationComponent },
  { path: "rejoin", component: JustOneRejoinComponent },
  { path: "sessionlobby", component: JustOneSessionLobbyComponent },
  { path: "waitingforgame", component: JustOneRoundWaitingComponent },
  { path: "round/waitingforround", component: JustOneWaitingForRoundComponent },

  { path: "round/submitclue", component: JustOneSubmitClueComponent },
  { path: "round/passiveplayerwaiting", component: JustOnePassivePlayerWaitingForCluesComponent },
  { path: "round/cluevote", component: JustOneClueVoteComponent },
  { path: "round/passiveplayerwaitingforvotes", component: JustOnePassivePlayerWaitingForClueVoteComponent },
  { path: "round/passiveplayerwaitingforactiveplayer", component: JustOnePassivePlayerWaitingForActivePlayerComponent },
  { path: "round/passiveplayeroutcome", component: JustOnePassivePlayerRoundOutcomeComponent },
  { path: "round/passiveplayeroutcomevote", component: JustOnePassivePlayerOutcomeVoteComponent },
  { path: "round/passiveplayerwaitingforoutcomevotes", component: JustOnePassivePlayerWaitingForOutcomeVoteComponent },


  { path: "round/playerwaiting", component: JustOneActivePlayerWaitingForCluesComponent },
  { path: "round/activeplayerwaitingforcluevotes", component: JustOneActivePlayerWaitingForClueVoteComponent },
  { path: "round/activeplayerguess", component: JustOneActivePlayerGuess },
  { path: "round/activeplayerwaitingforoutcomevotes", component: JustOneActivePlayerWaitingForOutcomeVoteComponent },
  { path: "round/activeplayeroutcome", component: JustOneActivePlayerRoundOutcomeComponent },

]

@NgModule({
  declarations: [

    TristateSwitch,

    JustOneLandingComponent,
    JustOneDeclarationComponent,
    JustOneRejoinComponent,

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
    JustOneWaitingForRoundComponent,

    JustOnePlayerWaitingComponent,
    JustOneClueListComponent
  ],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    FormsModule,
    NavbarsModule,
    SharedModule
  ],
  exports: [
    RouterModule
  ],
  entryComponents: [
    JustOneNavbarHeaderComponent,
    JustOneLandingComponent
  ]
})
export class JustOneModule { }
