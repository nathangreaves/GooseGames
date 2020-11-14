import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


import { SharedModule } from '../shared.module';
import { LetterJamComponent } from './letter-jam.component';
import { LetterJamContentDirective } from './scaffolding/content';
import { LetterJamLobbyComponent } from './lobby/lobby.component';
import { LetterJamLandingComponent } from './landing/landing.component';
import { LetterJamSubmitWordComponent } from './submit-word/submit-word.component';
import { LetterJamWaitingForFirstRoundComponent } from './waiting-for-first-round/waiting-for-first-round.component';
import { LetterJamTableComponent } from './table/table.component';
import { TableViewComponent } from './table/table-view/table-view.component';


const routes: Routes = [
  { path: '', component: LetterJamComponent },
  { path: ':id', component: LetterJamComponent },
]

@NgModule({
  declarations: [
    LetterJamComponent,
    LetterJamContentDirective,
    LetterJamLobbyComponent,
    LetterJamLandingComponent,
    LetterJamSubmitWordComponent,
    LetterJamWaitingForFirstRoundComponent,
    LetterJamTableComponent,
    TableViewComponent
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
    LetterJamLandingComponent,
    LetterJamLobbyComponent,
    LetterJamSubmitWordComponent,
    LetterJamWaitingForFirstRoundComponent,
    LetterJamTableComponent
  ]
})
export class LetterJamModule { }
