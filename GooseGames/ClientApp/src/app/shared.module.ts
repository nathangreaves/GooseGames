import { NgModule } from '@angular/core';
import { GlobalLobbyHubComponent } from './components/lobby/hub';
import { LobbyComponent } from './components/lobby/lobby';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmojiComponent } from './components/emoji/emoji.component';
import { PlayerWaitingComponent } from './components/playerwaiting/player-waiting.component';

@NgModule({
  declarations: [
    LobbyComponent,
    GlobalLobbyHubComponent,
    EmojiComponent,
    PlayerWaitingComponent
  ],
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    LobbyComponent,
    GlobalLobbyHubComponent,
    EmojiComponent,
    PlayerWaitingComponent
  ],
  entryComponents: [
  ]
})
export class SharedModule { }
