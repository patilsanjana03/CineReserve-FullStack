import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-seat-selection',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './seat-selection.html'
})
export class SeatSelectionComponent implements OnInit {
  rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];
  seatNumbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
  vipRows = ['G', 'H'];

  showtime: any = null;
  bookedSeats: string[] = [];
  selectedSeats: string[] = [];
  loading = true;
  booking = false;
  bookingResult: any = null;
  bookingError = '';
  showtimeId = 0;

  basePrice = 0;
  totalCost = 0;
  isLoggedIn = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public api: ApiService
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.api.isLoggedIn();
    this.showtimeId = parseInt(this.route.snapshot.paramMap.get('showtimeId') ?? '0');

    forkJoin({
      showtime: this.api.getShowtimeById(this.showtimeId),
      booked: this.api.getBookedSeats(this.showtimeId)
    }).subscribe({
      next: (data) => {
        this.showtime = data.showtime;
        this.bookedSeats = data.booked;
        this.basePrice = data.showtime.basePrice;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        alert('Could not load seat data. Check API connection.');
      }
    });
  }

  getSeatStatus(row: string, seat: number): 'available' | 'selected' | 'booked' {
    const key = `${row}-${seat}`;
    if (this.bookedSeats.includes(key)) return 'booked';
    if (this.selectedSeats.includes(key)) return 'selected';
    return 'available';
  }

  isVip(row: string): boolean {
    return this.vipRows.includes(row);
  }

  toggleSeat(row: string, seat: number): void {
    const status = this.getSeatStatus(row, seat);
    if (status === 'booked') return;

    const key = `${row}-${seat}`;
    const idx = this.selectedSeats.indexOf(key);

    if (idx > -1) {
      this.selectedSeats.splice(idx, 1);
    } else {
      this.selectedSeats.push(key);
    }

    this.calculateTotal();
  }

  getSeatPrice(row: string): number {
    return this.isVip(row) ? this.basePrice * 1.5 : this.basePrice;
  }

  calculateTotal(): void {
    this.totalCost = this.selectedSeats.reduce((sum, key) => {
      const row = key.split('-')[0];
      return sum + this.getSeatPrice(row);
    }, 0);
  }

  book(): void {
    if (!this.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }
    if (this.selectedSeats.length === 0) return;

    this.booking = true;
    this.bookingError = '';

    const payload = {
      showtimeId: this.showtimeId,
      seats: this.selectedSeats.map(key => {
        const parts = key.split('-');
        return { rowLetter: parts[0], seatNumber: parseInt(parts[1]) };
      })
    };

    this.api.bookSeats(payload).subscribe({
      next: (res) => {
        this.bookingResult = res;
        this.bookedSeats = [...this.bookedSeats, ...this.selectedSeats];
        this.selectedSeats = [];
        this.totalCost = 0;
        this.booking = false;
        localStorage.setItem('balance', res.remainingBalance.toString());
      },
      error: (err) => {
        this.bookingError = err.error?.message ?? 'Booking failed. Please try again.';
        this.booking = false;
      }
    });
  }

  get availableCount(): number {
    return (this.rows.length * this.seatNumbers.length) - this.bookedSeats.length;
  }

  get bookedCount(): number {
    return this.bookedSeats.length;
  }
}