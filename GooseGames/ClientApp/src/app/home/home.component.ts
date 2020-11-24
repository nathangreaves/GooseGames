import { Component, OnInit } from '@angular/core';
import { NavbarService } from '../../services/navbar';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit {
  _navbarService: NavbarService;
  constructor(navbarService: NavbarService) {
    this._navbarService = navbarService;

  }

  ngOnInit(): void {
    this._navbarService.reset();
    this._navbarService.setReadOnly(false);
    this._navbarService.setAreaTitle('');
    this._navbarService.setAreaContent(null);
  }
}
