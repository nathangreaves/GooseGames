import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


import { SharedModule } from '../shared.module';
import { AvalonComponent } from './avalon.component';
import { AvalonContentDirective } from './scaffolding/content';
import { AvalonLobbyComponent } from './lobby/lobby.component';
import { AvalonLandingComponent } from './landing/landing.component';
import { AvalonRoleInfoComponent } from './role-info/role-info.component';
import { AvalonTableComponent } from './table/table.component';


const routes: Routes = [
  { path: '', component: AvalonComponent },
  { path: ':id', component: AvalonComponent },
]

@NgModule({
  declarations: [
    AvalonComponent,
    AvalonContentDirective,
    AvalonLobbyComponent,
    AvalonLandingComponent,
    AvalonRoleInfoComponent,
    AvalonTableComponent
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
    AvalonLandingComponent,
    AvalonLobbyComponent,
    AvalonTableComponent
  ]
})
export class AvalonModule { }
