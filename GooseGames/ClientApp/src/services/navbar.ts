import { Injectable } from '@angular/core';
import { NavbarHeader, NavbarHeaderEnum } from '../app/nav-menu/navbar-header';
import { JustOneNavbarHeaderComponent } from '../app/justone/navbarheader.component';
import * as _ from 'lodash';
import { NavMenuComponent } from '../app/nav-menu/nav-menu.component';

@Injectable()
export class NavbarService {

  isReadOnly: boolean;
  AreaTitle: string;
  Navbar: NavMenuComponent;

  RegisteredNavbarHeaderComponents: NavbarHeader[] = [
    new NavbarHeader(NavbarHeaderEnum.JustOne, JustOneNavbarHeaderComponent),
  ];

  constructor() {
    this.isReadOnly = !!localStorage.getItem('navbar-isReadOnly');
    this.AreaTitle = localStorage.getItem('navbar-areaTitle');
  }

  reset() {
    this.setReadOnly(false);
    this.setAreaTitle('');
    this.setAreaContent(null);
  }

  setReadOnly(readOnly: boolean) {
    this.isReadOnly = readOnly;
    localStorage.setItem('navbar-isReadOnly', readOnly ? "true" : "");
  }
  setAreaTitle(title: string) {
    this.AreaTitle = title;
    localStorage.setItem('navbar-areaTitle', title);
  }
  setAreaContent(navbarHeader: NavbarHeaderEnum) {
    localStorage.setItem('navbar-areaContent', NavbarHeaderEnum[navbarHeader]);
    var navbarComponent = this.getNavbarComponentFromEnum(navbarHeader);
    this.Navbar.setNavbarHeader(navbarComponent);
  }

  getNavbarComponent(): NavbarHeader {
    var navbarHeaderEnum = NavbarHeaderEnum[localStorage.getItem('navbar-areaContent')];
    return this.getNavbarComponentFromEnum(navbarHeaderEnum);
  }

  getNavbarComponentFromEnum(enumValue: NavbarHeaderEnum): NavbarHeader {
    if (enumValue === null || enumValue === undefined) {
      return null;
    }

    return _.find(this.RegisteredNavbarHeaderComponents, c => c.Key == enumValue)
  }
}
