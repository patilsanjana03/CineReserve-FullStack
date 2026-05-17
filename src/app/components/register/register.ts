import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html'
})
export class RegisterComponent {
  username = '';
  email = '';
  password = '';
  error = '';
  success = '';
  loading = false;

  constructor(private api: ApiService, private router: Router) {}

  register(): void {
    if (!this.username || !this.email || !this.password) { this.error = 'Fill in all fields'; return; }
    this.loading = true;
    this.error = '';
    this.success = '';

    this.api.register({ username: this.username, email: this.email, password: this.password }).subscribe({
      next: () => {
        this.success = 'Registration successful! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Registration failed';
        this.loading = false;
      }
    });
  }
}