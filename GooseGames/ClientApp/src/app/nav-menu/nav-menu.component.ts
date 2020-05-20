import { Component, ViewChild, ComponentFactoryResolver, OnInit } from '@angular/core';
import { NavbarService } from '../../services/navbar';
import { NavbarHeaderDirective } from './navbar-header-directive';
import { INavbarHeaderComponent } from './navbar-header-content';
import { NavbarHeader } from './navbar-header';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  NavbarService: NavbarService;

  @ViewChild(NavbarHeaderDirective, { static: true }) navbarHeaderHost: NavbarHeaderDirective;
    ComponentSet: boolean;

  constructor(navbarService: NavbarService, private componentFactoryResolver: ComponentFactoryResolver) {
    this.NavbarService = navbarService;

    this.NavbarService.Navbar = this;
  }
  ngOnInit(): void {
    var navbarComponent = this.NavbarService.getNavbarComponent();
    this.setNavbarHeader(navbarComponent);
  }

  setNavbarHeader(navbarComponent: NavbarHeader) {

    const viewContainerRef = this.navbarHeaderHost.viewContainerRef;
    viewContainerRef.clear();
    
    if (navbarComponent == null || navbarComponent == undefined) {
      this.ComponentSet = false;
    }
    else {
      const componentFactory = navbarComponent ? this.componentFactoryResolver.resolveComponentFactory(navbarComponent.component) : null;

      if (componentFactory) {

        this.ComponentSet = true;

        const componentRef = viewContainerRef.createComponent(componentFactory);
        (<INavbarHeaderComponent>componentRef.instance) = navbarComponent;
      }
    }


  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
