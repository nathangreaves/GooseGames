import { NgModule } from '@angular/core';
import { GlobalLobbyHubComponent } from './components/lobby/hub';
import { LobbyComponent } from './components/lobby/lobby';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { EmojiModule } from '@ctrl/ngx-emoji-mart/ngx-emoji';

@NgModule({
  declarations: [
    LobbyComponent,
    GlobalLobbyHubComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    PickerModule,
    EmojiModule
  ],
  exports: [
    LobbyComponent,
    GlobalLobbyHubComponent
  ],
  entryComponents: [

  ]
})
export class SharedModule { }
