import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NavbarService } from '../services/navbar';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { TristateSwitch } from '../assets/tristate-switch.component'

import { JustOneLandingComponent } from './justone/landing.component';
import { JustOneNavbarHeaderComponent } from './justone/navbarheader.component';
import { JustOneDeclarationComponent } from './justone/declaration.component';
import { JustOneRejoinComponent } from './justone/rejoin.component';

import { JustOneNewPlayerDetailsComponent } from './justone/newplayerdetails.component';
import { JustOneSessionLobbyComponent } from './justone/sessionlobby.component';
import { JustOneRoundWaitingComponent } from './justone/round/waiting.component';
import { JustOneWaitingForRoundComponent } from './justone/round/waitingforround.component';

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
import { NavbarHeaderDirective } from './nav-menu/navbar-header-directive';


import { FujiSessionComponent } from './fujiflush/session.component'
import { FujiLandingComponent } from './fujiflush/landing.component';
import { FujiDisclaimerComponent } from './fujiflush/disclaimer.component';
import { FujiNewPlayerDetailsComponent } from './fujiflush/newplayerdetails.component';
import { FujiSessionLobbyComponent } from './fujiflush/sessionlobby.component';
import { FujiWaitingComponent } from './fujiflush/waiting.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    NavbarHeaderDirective,
    HomeComponent,
    JustOneNavbarHeaderComponent,
    TristateSwitch,

    JustOneLandingComponent,
    JustOneDeclarationComponent,
    JustOneRejoinComponent,


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
    JustOneWaitingForRoundComponent,

    JustOnePlayerWaitingComponent,
    JustOneClueListComponent,

    FujiLandingComponent,
    FujiDisclaimerComponent,
    FujiNewPlayerDetailsComponent,
    FujiSessionLobbyComponent,
    FujiWaitingComponent,
    FujiSessionComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'justone', component: JustOneLandingComponent },
      { path: 'fujiflush', component: FujiLandingComponent },

      { path: "justone/disclaimer", component: JustOneDeclarationComponent },
      { path: "justone/rejoin", component: JustOneRejoinComponent },
      { path: "justone/newplayer", component: JustOneNewPlayerDetailsComponent },
      { path: "justone/sessionlobby", component: JustOneSessionLobbyComponent },
      { path: "justone/waitingforgame", component: JustOneRoundWaitingComponent },
      { path: "justone/round/waitingforround", component: JustOneWaitingForRoundComponent },

      { path: "justone/round/submitclue", component: JustOneSubmitClueComponent },
      { path: "justone/round/passiveplayerwaiting", component: JustOnePassivePlayerWaitingForCluesComponent },
      { path: "justone/round/cluevote", component: JustOneClueVoteComponent },
      { path: "justone/round/passiveplayerwaitingforvotes", component: JustOnePassivePlayerWaitingForClueVoteComponent },
      { path: "justone/round/passiveplayerwaitingforactiveplayer", component: JustOnePassivePlayerWaitingForActivePlayerComponent },
      { path: "justone/round/passiveplayeroutcome", component: JustOnePassivePlayerRoundOutcomeComponent },
      { path: "justone/round/passiveplayeroutcomevote", component: JustOnePassivePlayerOutcomeVoteComponent },
      { path: "justone/round/passiveplayerwaitingforoutcomevotes", component: JustOnePassivePlayerWaitingForOutcomeVoteComponent },


      { path: "justone/round/playerwaiting", component: JustOneActivePlayerWaitingForCluesComponent },
      { path: "justone/round/activeplayerwaitingforcluevotes", component: JustOneActivePlayerWaitingForClueVoteComponent },
      { path: "justone/round/activeplayerguess", component: JustOneActivePlayerGuess },
      { path: "justone/round/activeplayerwaitingforoutcomevotes", component: JustOneActivePlayerWaitingForOutcomeVoteComponent },
      { path: "justone/round/activeplayeroutcome", component: JustOneActivePlayerRoundOutcomeComponent },

      { path: 'fujiflush/disclaimer', component: FujiDisclaimerComponent },
      { path: 'fujiflush/newplayer', component: FujiNewPlayerDetailsComponent },
      { path: 'fujiflush/sessionlobby', component: FujiSessionLobbyComponent },
      { path: 'fujiflush/waiting', component: FujiWaitingComponent },
      { path: 'fujiflush/session', component: FujiSessionComponent },
    ])
  ],
  entryComponents: [
    JustOneNavbarHeaderComponent
  ],
  providers: [
    NavbarService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
