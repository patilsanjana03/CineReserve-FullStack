import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html'
})
export class NavbarComponent implements OnInit {
  isLoggedIn = false;
  isAdmin = false;
  username = '';
  balance = 0;

  constructor(private api: ApiService, private router: Router) {}

  ngOnInit(): void { 
    this.refresh(); 
  }

  refresh(): void {
    this.isLoggedIn = this.api.isLoggedIn();
    this.username = this.api.getUsername();
    this.balance = this.api.getBalance();
    this.isAdmin = this.api.getRole().toLowerCase() === 'admin';
  }

  logout(): void {
    this.api.logout();
    this.isLoggedIn = false;
    this.isAdmin = false;
    this.router.navigate(['/']);
  }
}