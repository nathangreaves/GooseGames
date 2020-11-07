import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { FujiSessionComponent } from './session.component'
import { FujiLandingComponent } from './landing.component';
import { FujiDisclaimerComponent } from './disclaimer.component';
import { FujiNewPlayerDetailsComponent } from './newplayerdetails.component';
import { FujiSessionLobbyComponent } from './sessionlobby.component';
import { FujiWaitingComponent } from './waiting.component';

import { SharedModule } from '../shared.module';

const routes: Routes = [
  { path: '', component: FujiLandingComponent },
  { path: 'disclaimer', component: FujiDisclaimerComponent },
  { path: 'newplayer', component: FujiNewPlayerDetailsComponent },
  { path: 'sessionlobby', component: FujiSessionLobbyComponent },
  { path: 'waiting', component: FujiWaitingComponent },
  { path: 'session', component: FujiSessionComponent }
]

@NgModule({
  declarations: [
    FujiLandingComponent,
    FujiDisclaimerComponent,
    FujiNewPlayerDetailsComponent,
    FujiSessionLobbyComponent,
    FujiWaitingComponent,
    FujiSessionComponent
  ],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    FormsModule,
    SharedModule
  ],
  exports: [
    RouterModule
  ],
  entryComponents: [
    FujiLandingComponent
  ]
})
export class FujiFlushModule { }
