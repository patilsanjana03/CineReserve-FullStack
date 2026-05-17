import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-bookings.html'
})
export class MyBookingsComponent implements OnInit {
  bookings: any[] = [];
  loading = true;

  constructor(private api: ApiService, private router: Router) {}

  ngOnInit(): void {
    if (!this.api.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.api.getMyBookings().subscribe({
      next: (data) => { this.bookings = data; this.loading = false; },
      error: () => this.loading = false
    });
  }
}