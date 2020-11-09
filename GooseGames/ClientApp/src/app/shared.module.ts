import { NgModule } from '@angular/core';
import { GlobalLobbyHubComponent } from './components/lobby/hub';
import { LobbyComponent } from './components/lobby/lobby';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmojiComponent } from './components/emoji/emoji.component';

@NgModule({
  declarations: [
    LobbyComponent,
    GlobalLobbyHubComponent,
    EmojiComponent
  ],
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    LobbyComponent,
    GlobalLobbyHubComponent,
    EmojiComponent
  ],
  entryComponents: [
  ]
})
export class SharedModule { }
