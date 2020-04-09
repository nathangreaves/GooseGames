import { Type } from '@angular/core';

export class NavbarHeader {
  constructor(public Key: NavbarHeaderEnum, public component: Type<any>) { }
}

export enum NavbarHeaderEnum {
  JustOne
}
