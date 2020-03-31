import { Injectable } from '@angular/core';

@Injectable()
export class NavbarService {

  isReadOnly: boolean;
  AreaTitle: string;

  constructor() {
    this.isReadOnly = !!localStorage.getItem('navbar-isReadOnly');
    this.AreaTitle = localStorage.getItem('navbar-areaTitle');
  }

  setReadOnly(readOnly: boolean) {
    this.isReadOnly = readOnly;
    localStorage.setItem('navbar-isReadOnly', readOnly ? "true" : "");
  }
  setAreaTitle(title: string) {
    this.AreaTitle = title;
    localStorage.setItem('navbar-areaTitle', title);
  }
}
