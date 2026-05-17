import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html'
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(private api: ApiService, private router: Router) {}

  login(): void {
    if (!this.email || !this.password) { this.error = 'Fill in all fields'; return; }
    this.loading = true;
    this.error = '';

    this.api.login({ email: this.email, password: this.password }).subscribe({
      next: (res: any) => {
        localStorage.setItem('token', res.token);
        localStorage.setItem('username', res.username);
        localStorage.setItem('role', res.role);
        localStorage.setItem('balance', res.creditBalance.toString());
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.error = err.status === 401 ? 'Invalid email or password' : 'Login failed';
        this.loading = false;
      }
    });
  }
}