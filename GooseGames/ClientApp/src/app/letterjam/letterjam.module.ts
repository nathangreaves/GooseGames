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
import { LetterJamTableViewComponent } from './table/table-view/table-view.component';
import { LetterJamProposedCluesComponent } from './table/proposed-clues/proposed-clues.component';
import { LetterJamProposeClueComponent } from './table/propose-clue/propose-clue.component';
import { LetterJamClueComponent } from './table/clue/clue.component';
import { LetterJamProposedClueModalComponent } from './table/clue-modal/clue-modal.component';
import { LetterJamMyJamComponent } from './table/my-jam/my-jam.component';
import { LetterJamMyLettersComponent } from './table/my-jam/my-letters/my-letters.component';
import { LetterJamBonusLetterGuessedComponent } from './table/bonus-letter-guessed/bonus-letter-guessed.component';
import { LetterJamGameEndComponent } from './game-end/game-end.component';


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
    LetterJamTableViewComponent,
    LetterJamProposedCluesComponent,
    LetterJamProposeClueComponent,
    LetterJamProposedClueModalComponent,
    LetterJamClueComponent,
    LetterJamMyJamComponent,
    LetterJamMyLettersComponent,
    LetterJamBonusLetterGuessedComponent,
    LetterJamGameEndComponent
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
    LetterJamTableComponent,
    LetterJamGameEndComponent
  ]
})
export class LetterJamModule { }
