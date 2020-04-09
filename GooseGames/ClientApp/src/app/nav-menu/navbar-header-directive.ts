import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[navbar-header]',
})
export class NavbarHeaderDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}
