import { NgModule } from '@angular/core';
import { GlobalLobbyHubComponent } from './components/lobby/hub';
import { LobbyComponent } from './components/lobby/lobby';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    LobbyComponent,
    GlobalLobbyHubComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
  ],
  exports: [
    LobbyComponent,
    GlobalLobbyHubComponent
  ],
  entryComponents: [

  ]
})
export class SharedModule { }
