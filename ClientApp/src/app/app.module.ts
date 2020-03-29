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
import { JustOneActivePlayerWaitingForCluesComponent } from './justone/round/activeplayerwaitingforclues.component';


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
    JustOneActivePlayerWaitingForCluesComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'justone', component: JustOneLandingComponent},
      { path: PlayerStatusRoutesMap[PlayerStatus.New], component: JustOneNewPlayerDetailsComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.InLobby], component: JustOneSessionLobbyComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.RoundWaiting], component: JustOneRoundWaitingComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.PassivePlayerClue], component: JustOneSubmitClueComponent },
      { path: PlayerStatusRoutesMap[PlayerStatus.ActivePlayerWaitingForClues], component: JustOneActivePlayerWaitingForCluesComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
