import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NavbarService } from '../services/navbar';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { NavbarHeaderDirective } from './nav-menu/navbar-header-directive';

import { CodenamesLandingComponent } from './codenames/landing.component';
import { BigCodenamesSessionComponent, NormalCodenamesSessionComponent } from './codenames/session.component';
import { JustOneNavbarHeaderComponent } from './justone/navbarheader.component';
import { NavbarsModule } from './navbars.module';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    NavbarHeaderDirective,
    HomeComponent,
    CodenamesLandingComponent,
    BigCodenamesSessionComponent,
    NormalCodenamesSessionComponent    
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    NavbarsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'justone', loadChildren: () => import('./justone/justone.module').then(m => m.JustOneModule) },
      { path: 'fujiflush', loadChildren: () => import('./fujiflush/fujiflush.module').then(m => m.FujiFlushModule) },
      { path: 'codenames', component: CodenamesLandingComponent },
      { path: 'werewords', loadChildren: () => import('./werewords/werewords.module').then(m => m.WerewordsModule) },

      { path: 'codenames/:id', component: NormalCodenamesSessionComponent },
      { path: 'codenames/big/:id', component: BigCodenamesSessionComponent },


    ])
  ],
  entryComponents: [
    JustOneNavbarHeaderComponent
  ],
  providers: [
    NavbarService
  ],
  bootstrap: [AppComponent],
  exports: [
  ]
})
export class AppModule { }
