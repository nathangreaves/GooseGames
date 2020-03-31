import { Component } from '@angular/core';
import { NavbarService } from '../../services/navbar';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
    NavbarService: NavbarService;

  constructor(navbarService: NavbarService) {
    this.NavbarService = navbarService;
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
