import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  template: `
    <app-navbar></app-navbar>
    <router-outlet></router-outlet>
    <footer class="text-center py-3 mt-5" style="background:#0d0d2b; color:#888;">
      <small>© 2026 CineReserve — HCL Hackathon</small>
    </footer>
  `
})
export class AppComponent {}