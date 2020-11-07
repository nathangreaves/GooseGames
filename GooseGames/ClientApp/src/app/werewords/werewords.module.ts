import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { WerewordsComponent } from './werewords.component';

import { WerewordsWaitingForPlayerActionComponent } from './common/waitingforplayeraction';

import { WerewordsNightSecretWordComponent } from './night/secret-word';
import { WerewordsNightSecretRoleComponent } from './night/secret-role';
import { WerewordsNightMayorSecretWordComponent } from './night/mayor-secret-word';
import { WerewordsNightWakingComponent } from './night/waking';
import { WerewordsDayComponent } from './day/day';
import { WerewordsDayOutcomeComponent } from './day/outcome';
import { WerewordsLandingComponent } from './lobby/landing';
import { WerewordsNewPlayerDetailsComponent } from './lobby/newplayer';
import { WerewordsLobbyComponent } from './lobby/lobby';
import { WerewordsContentDirective } from './scaffolding/content';
import { SharedModule } from '../shared.module';


const routes: Routes = [
  { path: '', component: WerewordsComponent },
  { path: ':id', component: WerewordsComponent },
]

@NgModule({
  declarations: [
    WerewordsComponent,
    WerewordsContentDirective,
    WerewordsWaitingForPlayerActionComponent,

    WerewordsLandingComponent,
    WerewordsNewPlayerDetailsComponent,
    WerewordsLobbyComponent,

    WerewordsNightSecretRoleComponent,
    WerewordsNightSecretWordComponent,
    WerewordsNightMayorSecretWordComponent,
    WerewordsNightWakingComponent,
    WerewordsDayComponent,
    WerewordsDayOutcomeComponent
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
    WerewordsNightSecretRoleComponent,
    WerewordsNightSecretWordComponent,
    WerewordsNightMayorSecretWordComponent,
    WerewordsNightWakingComponent,
    WerewordsDayComponent,
    WerewordsDayOutcomeComponent,

    WerewordsLandingComponent,
    WerewordsNewPlayerDetailsComponent,
    WerewordsLobbyComponent
  ]
})
export class WerewordsModule { }
