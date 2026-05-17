import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './home.html'
})
export class HomeComponent implements OnInit {
  movies: any[] = [];
  showtimes: { [movieId: number]: any[] } = {};
  loading = true;
  selectedMovieId: number | null = null;
  searchQuery: string = '';
  popupMovie: any = null;

  constructor(public api: ApiService) {}

  ngOnInit(): void {
    this.api.getMovies().subscribe({
      next: (movies) => {
        this.movies = movies;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        alert('Could not connect to API. Is the .NET server running?');
      }
    });
  }

  get filteredMovies(): any[] {
    if (!this.searchQuery.trim()) { return this.movies; }
    return this.movies.filter(movie => 
      movie.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
      movie.genre.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  openDetailsPopup(movie: any): void {
    this.popupMovie = movie;
  }

  closeDetailsPopup(): void {
    this.popupMovie = null;
  }

  selectMovie(movieId: number): void {
    if (this.selectedMovieId === movieId) {
      this.selectedMovieId = null;
      return;
    }
    this.selectedMovieId = movieId;
    if (!this.showtimes[movieId]) {
      this.api.getShowtimes(movieId).subscribe({
        next: (data) => this.showtimes[movieId] = data
      });
    }
  }

  formatTime(timeStr: string): string {
    if (!timeStr) return '';
    const [h, m] = timeStr.split(':');
    const hour = parseInt(h);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    const hour12 = hour % 12 || 12;
    return `${hour12}:${m} ${ampm}`;
  }

  formatDate(dateStr: string): string {
    const d = new Date(dateStr);
    return d.toLocaleDateString('en-IN', { weekday: 'short', month: 'short', day: 'numeric' });
  }

  // CORRECTED: Now fully maps custom description tracking values directly from your input elements
  addMovieLive(title: string, genre: string, duration: string, rating: string, description: string): void {
    if (!title || !genre) { 
      alert('Please enter at least a title and genre!'); 
      return; 
    }
    
    const formattedKeyword = encodeURIComponent(title.trim().toLowerCase());
    const livePosterUrl = `https://images.unsplash.com/photo-1536440136628-849c177e76a1?auto=format&fit=crop&w=500&q=80&sig=${formattedKeyword}`;

    const syntheticPayload = {
      title: title,
      description: description.trim() || 'No synopsis provided for this screening collection record.',
      genre: genre,
      duration: parseInt(duration) || 120,
      rating: parseFloat(rating) || 7.5,
      posterUrl: livePosterUrl
    };

    this.api.registerMovieMock(syntheticPayload).subscribe({
      next: (insertedMovie) => {
        this.movies.push(insertedMovie);
        alert(`Success! "${title}" was permanently saved to SQL Server with your custom synopsis.`);
      },
      error: () => alert('Failed to save movie data structure to C# backend controller.')
    });
  }

  scheduleShowtimeLive(movieIdStr: string, dateStr: string, timeStr: string, hall: string, priceStr: string): void {
    const movieId = parseInt(movieIdStr);
    const price = parseFloat(priceStr);
    if (!movieId || !dateStr || !timeStr || !hall || !price) { alert('Please fill parameters completely!'); return; }

    const showtimePayload = { movieId, showDate: dateStr, showTime: timeStr + ":00", hallName: hall, basePrice: price };
    this.api.addShowtimeLive(showtimePayload).subscribe({
      next: (insertedShowtime) => {
        alert('Success! New show schedule added.');
        if (this.showtimes[movieId]) this.showtimes[movieId].push(insertedShowtime);
        else this.showtimes[movieId] = [insertedShowtime];
      },
      error: () => alert('Failed to sync showtime slots.')
    });
  }
}